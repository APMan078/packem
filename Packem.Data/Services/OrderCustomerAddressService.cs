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
    public class OrderCustomerAddressService : IOrderCustomerAddressService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public OrderCustomerAddressService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<OrderCustomerAddressGetModel>> CreateOrderCustomerAddressAsync(AppState state, OrderCustomerAddressCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.OrderCustomerId is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressCreateModel.OrderCustomerId)} is required.");
                }
                else
                {
                    var exist = await _context.OrderCustomers
                        .AnyAsync(x => x.OrderCustomerId == model.OrderCustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(OrderCustomer)} not found.");
                    }
                }

                if (model.AddressType is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressCreateModel.AddressType)} is required.");
                }
                else
                {
                    if (!model.AddressType.IsValueExistInEnum())
                    {
                        return Result.Fail($"{nameof(OrderCustomerAddressType)} not found.");
                    }
                }

                if (!model.Address1.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressCreateModel.Address1)} is required.");
                }

                if (!model.StateProvince.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressCreateModel.StateProvince)} is required.");
                }

                if (!model.ZipPostalCode.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressCreateModel.ZipPostalCode)} is required.");
                }

                if (!model.Country.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressCreateModel.Country)} is required.");
                }

                var entity = new OrderCustomerAddress
                {
                    AddressType = model.AddressType.Value,
                    OrderCustomerId = model.OrderCustomerId,
                    Address1 = model.Address1,
                    City = model.City,
                    Address2 = model.Address2,
                    StateProvince = model.StateProvince,
                    ZipPostalCode = model.ZipPostalCode,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new OrderCustomerAddressGetModel
                {
                    OrderCustomerAddressId = entity.OrderCustomerAddressId,
                    AddressType = entity.AddressType,
                    OrderCustomerId = entity.OrderCustomerId.Value,
                    Address1 = entity.Address1,
                    Address2 = entity.Address2,
                    City = model.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    Country = entity.Country,
                    PhoneNumber = entity.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderCustomerAddressGetModel>> EditOrderCustomerAddressAsync(AppState state, OrderCustomerAddressEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.OrderCustomerAddressId is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressEditModel.OrderCustomerAddressId)} is required.");
                }

                var entity = await _context.OrderCustomerAddresses
                    .SingleOrDefaultAsync(x => x.OrderCustomerAddressId == model.OrderCustomerAddressId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerAddress)} not found.");
                }

                if (!model.Address1.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressEditModel.Address1)} is required.");
                }

                if (!model.StateProvince.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressEditModel.StateProvince)} is required.");
                }

                if (!model.ZipPostalCode.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressEditModel.ZipPostalCode)} is required.");
                }

                if (!model.Country.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressEditModel.Country)} is required.");
                }

                entity.Address1 = model.Address1;
                entity.Address2 = model.Address2;
                entity.City = model.City;
                entity.StateProvince = model.StateProvince;
                entity.ZipPostalCode = model.ZipPostalCode;
                entity.Country = model.Country;
                entity.PhoneNumber = model.PhoneNumber;

                await _context.SaveChangesAsync();

                return Result.Ok(new OrderCustomerAddressGetModel
                {
                    OrderCustomerAddressId = entity.OrderCustomerAddressId,
                    AddressType = entity.AddressType,
                    OrderCustomerId = entity.OrderCustomerId.Value,
                    Address1 = entity.Address1,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    Country = entity.Country,
                    PhoneNumber = entity.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderCustomerAddressGetModel>> DeleteOrderCustomerAddressAsync(AppState state, OrderCustomerAddressDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.OrderCustomerAddressId is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerAddressDeleteModel.OrderCustomerAddressId)} is required.");
                }
                else
                {
                    var exist = await _context.OrderCustomerAddresses
                        .AnyAsync(x => x.OrderCustomerAddressId == model.OrderCustomerAddressId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(OrderCustomerAddress)} not found.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.OrderCustomerAddresses
                            .Include(x => x.SaleOrderShippingAddresses)
                            .Include(x => x.SaleOrderBillingAddresses)
                            .AsSplitQuery()
                            .SingleOrDefaultAsync(x => x.OrderCustomerAddressId == model.OrderCustomerAddressId);

                        entity.Deleted = true;

                        if (entity.AddressType == OrderCustomerAddressType.ShippingAddress)
                        {
                            foreach (var x in entity.SaleOrderShippingAddresses)
                            {
                                x.ShippingAddressId = null;
                            }
                        }
                        else if (entity.AddressType == OrderCustomerAddressType.BillingAddress)
                        {
                            foreach (var x in entity.SaleOrderBillingAddresses)
                            {
                                x.BillingAddressId = null;
                            }
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return Result.Ok(new OrderCustomerAddressGetModel
                        {
                            OrderCustomerAddressId = entity.OrderCustomerAddressId,
                            AddressType = entity.AddressType,
                            OrderCustomerId = entity.OrderCustomerId.Value,
                            Address1 = entity.Address1,
                            Address2 = entity.Address2,
                            City = entity.City,
                            StateProvince = entity.StateProvince,
                            ZipPostalCode = entity.ZipPostalCode,
                            Country = entity.Country,
                            PhoneNumber = entity.PhoneNumber
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

        public async Task<Result<IEnumerable<OrderCustomerAddressGetModel>>> GetOrderCustomerAddressesAsync(AppState state, int orderCustomerId, OrderCustomerAddressType? addressType = null)
        {
            try
            {
                var query = _context.OrderCustomerAddresses
                    .AsQueryable();

                if (addressType is null)
                {
                    query = query.Where(x => x.OrderCustomerId == orderCustomerId);
                }
                else
                {
                    query = query.Where(x => x.OrderCustomerId == orderCustomerId
                        && x.AddressType == addressType.Value);
                }

                IEnumerable<OrderCustomerAddressGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new OrderCustomerAddressGetModel
                    {
                        OrderCustomerAddressId = x.OrderCustomerAddressId,
                        AddressType = x.AddressType,
                        OrderCustomerId = x.OrderCustomerId.Value,
                        Address1 = x.Address1,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        Country = x.Country,
                        PhoneNumber = x.PhoneNumber
                    })
                    .Where(x => x.OrderCustomerId == orderCustomerId)
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderCustomerAddressGetModel>> GetOrderCustomerAddressAsync(AppState state, int orderCustomerAddressId)
        {
            try
            {
                var model = await _context.OrderCustomerAddresses
                    .AsNoTracking()
                    .Select(x => new OrderCustomerAddressGetModel
                    {
                        OrderCustomerAddressId = x.OrderCustomerAddressId,
                        AddressType = x.AddressType,
                        OrderCustomerId = x.OrderCustomerId.Value,
                        Address1 = x.Address1,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        Country = x.Country,
                        PhoneNumber = x.PhoneNumber
                    })
                    .SingleOrDefaultAsync(x => x.OrderCustomerAddressId == orderCustomerAddressId);

                if (model == null)
                {
                    return Result.Fail($"{nameof(OrderCustomerAddress)} not found.");
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