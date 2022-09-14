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
    public class CustomerDeviceService : ICustomerDeviceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public CustomerDeviceService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<CustomerDeviceGetModel>> CreateCustomerDeviceAsync(AppState state, CustomerDeviceCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceCreateModel.CustomerId)} is required.");
                }

                var existCustomerId = await _context.Customers
                        .AnyAsync(x => x.CustomerId == model.CustomerId);

                if (!existCustomerId)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceCreateModel.CustomerLocationId)} is required.");
                }

                var customerLocationExist = await _context.CustomerLocations
                        .AnyAsync(x => x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == model.CustomerId);

                if (!customerLocationExist)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                if (!model.SerialNumber.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerDeviceGetModel.SerialNumber)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerDevices
                        .AnyAsync(x => x.SerialNumber.Trim().ToLower() == model.SerialNumber.Trim().ToLower()
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerDeviceGetModel.SerialNumber)} already exists.");
                    }
                }

                var entity = new CustomerDevice
                {
                    CustomerId = model.CustomerId,
                    CustomerLocationId = model.CustomerLocationId,
                    SerialNumber = model.SerialNumber,
                    AddedDateTime = DateTime.Now,
                    IsActive = true
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerDeviceGetModel
                {
                    CustomerDeviceId = entity.CustomerDeviceId,
                    CustomerId = entity.CustomerId.Value,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    SerialNumber = entity.SerialNumber,
                    AddedDateTime = entity.AddedDateTime,
                    IsActive = entity.IsActive,
                    LastLoginDateTime = entity.LastLoginDateTime
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerDeviceGetModel>> EditCustomerDeviceAsync(AppState state, CustomerDeviceEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerDeviceId is null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceEditModel.CustomerDeviceId)} is required.");
                }

                CustomerDevice entity;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    entity = await _context.CustomerDevices
                        .Include(x => x.CustomerLocation)
                        .SingleOrDefaultAsync(x => x.CustomerDeviceId == model.CustomerDeviceId
                            && x.CustomerLocation.CustomerId == state.CustomerId);
                }
                else
                {
                    entity = await _context.CustomerDevices
                        .SingleOrDefaultAsync(x => x.CustomerDeviceId == model.CustomerDeviceId);
                }

                if (entity is null)
                {
                    return Result.Fail($"{nameof(CustomerDevice)} not found.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceEditModel.CustomerLocationId)} is required.");
                }
                else
                {
                    var query = _context.CustomerLocations.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerLocationId == model.CustomerLocationId
                                && x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerLocationId == model.CustomerLocationId);
                    }

                    var customerLocationExist = await query
                        .AnyAsync();

                    if (!customerLocationExist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                if (!model.SerialNumber.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerDeviceEditModel.SerialNumber)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerDevices
                        .AnyAsync(x => x.SerialNumber.Trim().ToLower() == model.SerialNumber.Trim().ToLower()
                            && x.SerialNumber.Trim().ToLower() != entity.SerialNumber.Trim().ToLower()
                            && x.CustomerLocationId == entity.CustomerLocationId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerFacilityEditModel.Name)} is already exist.");
                    }
                }

                entity.CustomerLocationId = model.CustomerLocationId;
                entity.SerialNumber = model.SerialNumber;
                entity.IsActive = model.IsActive;

                if (!model.IsActive)
                {
                    entity.DeactivedDateTime = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerDeviceGetModel
                {
                    CustomerDeviceId = entity.CustomerDeviceId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    SerialNumber = entity.SerialNumber,
                    AddedDateTime = entity.AddedDateTime,
                    IsActive = entity.IsActive,
                    LastLoginDateTime = entity.LastLoginDateTime
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerDeviceGetModel>> EditCustomerDeviceIsActiveAsync(AppState state, CustomerDeviceIsActiveEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerDeviceId is null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceIsActiveEditModel.CustomerDeviceId)} is required.");
                }

                if (model.IsActive is null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceIsActiveEditModel.IsActive)} is required.");
                }

                var entity = await _context.CustomerDevices
                    .SingleOrDefaultAsync(x => x.CustomerDeviceId == model.CustomerDeviceId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(CustomerDevice)} not found.");
                }

                entity.IsActive = model.IsActive.Value;

                if (!entity.IsActive)
                {
                    entity.DeactivedDateTime = DateTime.Now;
                }
                else
                {
                    entity.DeactivedDateTime = null;
                }

                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerDeviceGetModel
                {
                    CustomerDeviceId = entity.CustomerDeviceId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    SerialNumber = entity.SerialNumber,
                    AddedDateTime = entity.AddedDateTime,
                    IsActive = entity.IsActive,
                    LastLoginDateTime = entity.LastLoginDateTime
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<CustomerDeviceGetModel>>> GetCustomerDevicesByCustomerIdAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Customers.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query
                        .Where(x => x.CustomerId == customerId
                            && x.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query
                        .Where(x => x.CustomerId == customerId);
                }

                var customerExist = await query
                    .AnyAsync();

                if (!customerExist)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                IEnumerable<CustomerDeviceGetModel> model = await _context.CustomerDevices
                    .AsNoTracking()
                    .Select(x => new CustomerDeviceGetModel
                    {
                        CustomerDeviceId = x.CustomerDeviceId,
                        CustomerId = x.CustomerId.Value,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        SerialNumber = x.SerialNumber,
                        AddedDateTime = x.AddedDateTime,
                        IsActive = x.IsActive,
                        LastLoginDateTime = x.LastLoginDateTime
                    })
                    .Where(x => x.CustomerId == customerId)
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<CustomerDeviceGetModel>>> GetCustomerDevicesByCustomerLocationIdAsync(AppState state, int customerLocationId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.CustomerLocations.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query
                        .Where(x => x.CustomerLocationId == customerLocationId
                            && x.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query
                        .Where(x => x.CustomerLocationId == customerLocationId);
                }

                var customerLocationExist = await query
                    .AnyAsync();

                if (!customerLocationExist)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                IEnumerable<CustomerDeviceGetModel> model = await _context.CustomerDevices
                    .AsNoTracking()
                    .Select(x => new CustomerDeviceGetModel
                    {
                        CustomerDeviceId = x.CustomerDeviceId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        SerialNumber = x.SerialNumber,
                        AddedDateTime = x.AddedDateTime,
                        IsActive = x.IsActive,
                        LastLoginDateTime = x.LastLoginDateTime
                    })
                    .Where(x => x.CustomerLocationId == customerLocationId)
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerDeviceGetModel>> GetCustomerDeviceAsync(AppState state, int customerDeviceId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var exist = await _context.CustomerDevices
                        .Include(x => x.CustomerLocation)
                        .AnyAsync(x => x.CustomerDeviceId == customerDeviceId
                            && x.CustomerLocation.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerDevice)} not found.");
                    }
                }

                var model = await _context.CustomerDevices
                    .AsNoTracking()
                    .Select(x => new CustomerDeviceGetModel
                    {
                        CustomerDeviceId = x.CustomerDeviceId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        SerialNumber = x.SerialNumber,
                        AddedDateTime = x.AddedDateTime,
                        IsActive = x.IsActive,
                        LastLoginDateTime = x.LastLoginDateTime
                    })
                    .SingleOrDefaultAsync(x => x.CustomerDeviceId == customerDeviceId);

                if (model is null)
                {
                    return Result.Fail($"{nameof(CustomerDevice)} not found.");
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
