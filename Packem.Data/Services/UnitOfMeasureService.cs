using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Entities;
using Packem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class UnitOfMeasureService : IUnitOfMeasureService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public UnitOfMeasureService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<IEnumerable<UnitOfMeasureGetModel>>> CreateUnitOfMeasureForCustomerAsync(AppState state, UnitOfMeasureForCustomerCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(UnitOfMeasureForCustomerCreateModel.CustomerId)} is required.");
                }
                else
                {
                    var query = _context.Customers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }

                if (model.UnitOfMeasureIds.Count() == 0)
                {
                    return Result.Fail($"{nameof(UnitOfMeasureForCustomerCreateModel.UnitOfMeasureIds)} is required.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var dto = new List<UnitOfMeasureGetModel>();
                        foreach (var x in model.UnitOfMeasureIds)
                        {
                            var uom = await _context.UnitOfMeasures
                                .AsNoTracking()
                                .SingleOrDefaultAsync(z => (z.UnitOfMeasureId == x && z.Type == UnitOfMeasureTypeEnum.Default)
                                    || (z.UnitOfMeasureId == x && z.Type == UnitOfMeasureTypeEnum.Custom && z.CustomerId == model.CustomerId));

                            if (uom is null)
                            {
                                return Result.Fail($"{nameof(UnitOfMeasure)} not found.");
                            }
                            else
                            {
                                var uomCustomer = await _context.UnitOfMeasureCustomers
                                    .SingleOrDefaultAsync(z => z.UnitOfMeasureId == x
                                        && z.CustomerId == model.CustomerId);

                                if (uomCustomer is null) // create
                                {
                                    uomCustomer = new UnitOfMeasureCustomer
                                    {
                                        UnitOfMeasureId = x,
                                        CustomerId = model.CustomerId,
                                    };
                                    _context.Add(uomCustomer);
                                    await _context.SaveChangesAsync();
                                }

                                dto.Add(new UnitOfMeasureGetModel
                                {
                                    UnitOfMeasureId = x,
                                    Code = uom.Code,
                                    Description = uom.Description,
                                    Type = uom.Type
                                });
                            }
                        }

                        await transaction.CommitAsync();

                        return Result.Ok(dto.AsEnumerable());
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ex = await _exceptionService.HandleExceptionAsync(ex);
                        return Result.Fail(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<UnitOfMeasureGetModel>> CreateCustomUnitOfMeasureForCustomerAsync(AppState state, CustomUnitOfMeasureForCustomerCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(UnitOfMeasureForCustomerCreateModel.CustomerId)} is required.");
                }
                else
                {
                    var query = _context.Customers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }

                if (!model.Code.HasValue())
                {
                    return Result.Fail($"{nameof(CustomUnitOfMeasureForCustomerCreateModel.Code)} is required.");
                }
                else
                {
                    var exist = await _context.UnitOfMeasures
                            .AnyAsync(x => (x.Code.Trim().ToLower() == model.Code.Trim().ToLower() && x.Type == UnitOfMeasureTypeEnum.Default)
                                || (x.Code.Trim().ToLower() == model.Code.Trim().ToLower() && x.Type == UnitOfMeasureTypeEnum.Custom && x.CustomerId == model.CustomerId));

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomUnitOfMeasureForCustomerCreateModel.Code)} is already exist.");
                    }
                }

                if (!model.Description.HasValue())
                {
                    return Result.Fail($"{nameof(CustomUnitOfMeasureForCustomerCreateModel.Description)} is required.");
                }
                else
                {
                    var exist = await _context.UnitOfMeasures
                            .AnyAsync(x => (x.Description.Trim().ToLower() == model.Description.Trim().ToLower() && x.Type == UnitOfMeasureTypeEnum.Default)
                                || (x.Description.Trim().ToLower() == model.Description.Trim().ToLower() && x.Type == UnitOfMeasureTypeEnum.Custom && x.CustomerId == model.CustomerId));

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomUnitOfMeasureForCustomerCreateModel.Description)} is already exist.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var uom = new UnitOfMeasure
                        {
                            CustomerId = model.CustomerId,
                            Code = model.Code,
                            Description = model.Description,
                            Type = UnitOfMeasureTypeEnum.Custom
                        };
                        _context.Add(uom);
                        await _context.SaveChangesAsync();

                        var uomCustomer = await _context.UnitOfMeasureCustomers
                            .SingleOrDefaultAsync(z => z.UnitOfMeasureId == uom.UnitOfMeasureId
                                && z.CustomerId == model.CustomerId);

                        if (uomCustomer is null) // create
                        {
                            uomCustomer = new UnitOfMeasureCustomer
                            {
                                UnitOfMeasureId = uom.UnitOfMeasureId,
                                CustomerId = model.CustomerId,
                            };
                            _context.Add(uomCustomer);
                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();

                        return Result.Ok(new UnitOfMeasureGetModel
                        {
                            UnitOfMeasureId = uom.UnitOfMeasureId,
                            Code = uom.Code,
                            Description = uom.Description,
                            Type = uom.Type
                        });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ex = await _exceptionService.HandleExceptionAsync(ex);
                        return Result.Fail(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<UnitOfMeasureGetModel>> DeleteCustomerUnitOfMeasureAsync(AppState state, CustomerUnitOfMeasureDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(CustomerUnitOfMeasureDeleteModel.CustomerId)} is required.");
                }
                else
                {
                    var query = _context.Customers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }

                if (model.UnitOfMeasureId is null)
                {
                    return Result.Fail($"{nameof(CustomerUnitOfMeasureDeleteModel.UnitOfMeasureId)} is required.");
                }
                else
                {
                    var query = _context.UnitOfMeasureCustomers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.UnitOfMeasureId == model.UnitOfMeasureId
                                && x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.UnitOfMeasureId == model.UnitOfMeasureId
                                && x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(UnitOfMeasure)} not found.");
                    }
                }

                var usedForItems = await _context.Items.Where(x => x.UnitOfMeasureId == model.UnitOfMeasureId 
                                                                && x.CustomerId == state.CustomerId 
                                                                && !x.Deleted).AnyAsync();
                if(usedForItems)
                {
                    return Result.Fail("Cannot remove UoM because it is set to a item in your account.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.UnitOfMeasureCustomers
                            .Include(x => x.UnitOfMeasure)
                                .SingleOrDefaultAsync(x => x.UnitOfMeasureId == model.UnitOfMeasureId
                                    && x.CustomerId == model.CustomerId);
                        entity.Deleted = true;
                        if (entity.UnitOfMeasure.CustomerId is not null)
                        {
                            entity.UnitOfMeasure.Deleted = true;
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return Result.Ok(new UnitOfMeasureGetModel
                        {
                            UnitOfMeasureId = entity.UnitOfMeasure.UnitOfMeasureId,
                            Code = entity.UnitOfMeasure.Code,
                            Description = entity.UnitOfMeasure.Description,
                            Type = entity.UnitOfMeasure.Type
                        });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ex = await _exceptionService.HandleExceptionAsync(ex);
                        return Result.Fail(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<UnitOfMeasureGetModel>>> GetDefaultUnitOfMeasuresAsync(AppState state, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.UnitOfMeasures
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.Type == UnitOfMeasureTypeEnum.Default
                        && (x.Code.Trim().ToLower().Contains(searchText)
                            || x.Description.Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    query = query.Where(x => x.Type == UnitOfMeasureTypeEnum.Default);
                }

                IEnumerable<UnitOfMeasureGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new UnitOfMeasureGetModel
                    {
                        UnitOfMeasureId = x.UnitOfMeasureId,
                        Code = x.Code,
                        Description = x.Description,
                        Type = x.Type
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(UnitOfMeasure)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<UnitOfMeasureGetModel>>> GetCustomerUnitOfMeasuresAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.UnitOfMeasureCustomers
                    .Include(x => x.UnitOfMeasure)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == customerId
                        && (x.UnitOfMeasure.Code.Trim().ToLower().Contains(searchText)
                            || x.UnitOfMeasure.Description.Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }

                IEnumerable<UnitOfMeasureGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new UnitOfMeasureGetModel
                    {
                        UnitOfMeasureId = x.UnitOfMeasure.UnitOfMeasureId,
                        Code = x.UnitOfMeasure.Code,
                        Description = x.UnitOfMeasure.Description,
                        Type = x.UnitOfMeasure.Type
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(UnitOfMeasure)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }
    }
}