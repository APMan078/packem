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
    public class OrderCustomerService : IOrderCustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public OrderCustomerService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<OrderCustomerGetModel>> CreateOrderCustomerAsync(AppState state, OrderCustomerCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerCreateModel.CustomerId)} is required.");
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

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerCreateModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.OrderCustomers
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.CustomerId == model.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(OrderCustomerCreateModel.Name)} is already exist.");
                    }
                }

                if (model.PaymentType is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerCreateModel.PaymentType)} is required.");
                }

                var entity = new OrderCustomer
                {
                    CustomerId = model.CustomerId,
                    Name = model.Name,
                    PaymentType = model.PaymentType.Value
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new OrderCustomerGetModel
                {
                    OrderCustomerId = entity.OrderCustomerId,
                    CustomerId = entity.CustomerId.Value,
                    Name = entity.Name,
                    PaymentType = entity.PaymentType,
                    LastOrderDate = entity.LastOrderDate
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderCustomerGetModel>> EditOrderCustomerAsync(AppState state, OrderCustomerEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.OrderCustomerId is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerEditModel.OrderCustomerId)} is required.");
                }

                OrderCustomer entity;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    entity = await _context.OrderCustomers
                        .SingleOrDefaultAsync(x => x.OrderCustomerId == model.OrderCustomerId
                            && x.CustomerId == state.CustomerId);
                }
                else
                {
                    entity = await _context.OrderCustomers
                        .SingleOrDefaultAsync(x => x.OrderCustomerId == model.OrderCustomerId);
                }

                if (entity is null)
                {
                    return Result.Fail($"{nameof(OrderCustomer)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(OrderCustomerEditModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.OrderCustomers
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.Name.Trim().ToLower() != entity.Name.Trim().ToLower()
                            && x.CustomerId == entity.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(OrderCustomerEditModel.Name)} is already exist.");
                    }
                }

                if (model.PaymentType is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerEditModel.PaymentType)} is required.");
                }

                entity.Name = model.Name;
                entity.PaymentType = model.PaymentType.Value;

                await _context.SaveChangesAsync();

                return Result.Ok(new OrderCustomerGetModel
                {
                    OrderCustomerId = entity.OrderCustomerId,
                    CustomerId = entity.CustomerId.Value,
                    Name = entity.Name,
                    PaymentType = entity.PaymentType,
                    LastOrderDate = entity.LastOrderDate
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderCustomerGetModel>> DeleteOrderCustomerAsync(AppState state, OrderCustomerDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerDeleteModel.CustomerId)} is required.");
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

                if (model.OrderCustomerId is null)
                {
                    return Result.Fail($"{nameof(OrderCustomerDeleteModel.OrderCustomerId)} is required.");
                }
                else
                {
                    var query = _context.OrderCustomers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.OrderCustomerId == model.OrderCustomerId
                                && x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.OrderCustomerId == model.OrderCustomerId
                                && x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(OrderCustomer)} not found.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.OrderCustomers
                            .Include(x => x.OrderCustomerAddresses)
                                .ThenInclude(x => x.SaleOrderShippingAddresses)
                            .Include(x => x.OrderCustomerAddresses)
                                .ThenInclude(x => x.SaleOrderBillingAddresses)
                            .AsSplitQuery()
                            .SingleOrDefaultAsync(x => x.OrderCustomerId == model.OrderCustomerId
                                && x.CustomerId == model.CustomerId);

                        entity.Deleted = true;

                        foreach (var x in entity.OrderCustomerAddresses)
                        {
                            x.Deleted = true;

                            if (x.AddressType == OrderCustomerAddressType.ShippingAddress)
                            {
                                foreach (var z in x.SaleOrderShippingAddresses)
                                {
                                    z.ShippingAddressId = null;
                                }
                            }
                            else if (x.AddressType == OrderCustomerAddressType.BillingAddress)
                            {
                                foreach (var z in x.SaleOrderBillingAddresses)
                                {
                                    z.BillingAddressId = null;
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return Result.Ok(new OrderCustomerGetModel
                        {
                            OrderCustomerId = entity.OrderCustomerId,
                            CustomerId = entity.CustomerId.Value,
                            Name = entity.Name,
                            PaymentType = entity.PaymentType,
                            LastOrderDate = entity.LastOrderDate
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

        public async Task<Result<IEnumerable<OrderCustomerGetModel>>> GetOrderCustomersAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var customers = await _context.OrderCustomers.Where(x => x.OrderCustomerAddresses
                                                                        .Any(a => a.AddressType == OrderCustomerAddressType.ShippingAddress)
                                                                   && x.OrderCustomerAddresses
                                                                        .Any(a => a.AddressType == OrderCustomerAddressType.BillingAddress)
                                                                   ).ToListAsync();

                IEnumerable<OrderCustomerGetModel> model = customers
                    .Select(x => new OrderCustomerGetModel
                    {
                        OrderCustomerId = x.OrderCustomerId,
                        CustomerId = x.CustomerId.Value,
                        Name = x.Name,
                        PaymentType = x.PaymentType,
                        LastOrderDate = x.LastOrderDate
                    })
                    .Where(x => x.CustomerId == customerId)
                    .ToList();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderCustomerGetModel>> GetOrderCustomerAsync(AppState state, int orderCustomerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var model = await _context.OrderCustomers
                    .AsNoTracking()
                    .Select(x => new OrderCustomerGetModel
                    {
                        OrderCustomerId = x.OrderCustomerId,
                        CustomerId = x.CustomerId.Value,
                        Name = x.Name,
                        PaymentType = x.PaymentType,
                        LastOrderDate = x.LastOrderDate
                    })
                    .SingleOrDefaultAsync(x => x.OrderCustomerId == orderCustomerId);

                if (model == null)
                {
                    return Result.Fail($"{nameof(OrderCustomer)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<OrderCustomerManagementGetModel>>> GetOrderCustomerManagementAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var customers = await _context.OrderCustomers
                    .Include(x => x.SaleOrders)
                        .ThenInclude(x => x.OrderLines)
                            .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.Inventory)
                    .AsNoTracking()
                    .Where(x => x.CustomerId == customerId)
                    .ToListAsync();


                var model = new List<OrderCustomerManagementGetModel>();
                foreach (var x in customers)
                {
                    var c = new OrderCustomerManagementGetModel();

                    var addresses = await _context.OrderCustomerAddresses.Where(a => a.OrderCustomerId == x.OrderCustomerId).ToListAsync();

                    c.OrderCustomerId = x.OrderCustomerId;
                    c.CustomerName = x.Name;
                    c.NoShippingAddresses = addresses.Where(a => a.AddressType == OrderCustomerAddressType.ShippingAddress).Count();
                    c.NoBillingAddresses = addresses.Where(a => a.AddressType == OrderCustomerAddressType.BillingAddress).Count();

                    model.Add(c);
                }

                return Result.Ok(model.AsEnumerable());

            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderCustomerDetailGetModel>> GetOrderCustomerDetailAsync(AppState state, int orderCustomerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var so = await _context.SaleOrders
                    .Include(x => x.OrderCustomer)
                    .Include(x => x.ShippingAddress)
                    .Include(x => x.BillingAddress)
                    .Where(x => x.OrderCustomerId == orderCustomerId)
                    .AsNoTracking()
                    .Select(x => new OrderCustomerDetailGetModel.CustomerDetail
                    {
                        OrderCustomerId = x.OrderCustomer.OrderCustomerId,
                        CustomerName = x.OrderCustomer.Name,
                        ShipToAddress1 = x.ShippingAddress.Address1,
                        ShipToAddress2 = x.ShippingAddress.Address2,
                        City = x.ShippingAddress.City,
                        ShipToStateProvince = x.ShippingAddress.StateProvince,
                        ShipToZipPostalCode = x.ShippingAddress.ZipPostalCode,
                        ShipToCountry = x.ShippingAddress.Country,
                        ShipToPhoneNumber = x.ShippingAddress.PhoneNumber,
                        BillToAddress1 = x.BillingAddress.Address1,
                        BillToAddress2 = x.BillingAddress.Address2,
                        BillToStateProvince = x.BillingAddress.StateProvince,
                        BillToZipPostalCode = x.BillingAddress.ZipPostalCode,
                        BillToCountry = x.BillingAddress.Country,
                        BillToPhoneNumber = x.BillingAddress.PhoneNumber
                    })
                    .ToListAsync();

                //sale order history
                var soh = await _context.SaleOrders
                    .Where(x => x.OrderCustomerId == orderCustomerId)
                    .AsNoTracking()
                    .Select(x => new OrderCustomerDetailGetModel.SaleOrder
                    {
                        SaleOrderId = x.SaleOrderId,
                        OrderNo = x.SaleOrderNo,
                        Status = x.Status.ToLabel(),
                        OrderDate = x.OrderDate,
                        PromisedDate = x.PromiseDate,
                        OrderQty = x.OrderQty,
                    })
                    .ToListAsync();

                var ol = await _context.OrderLines
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.InventoryBins)
                    .Where(x => soh.Select(z => z.SaleOrderId).Contains(x.SaleOrderId.Value))
                    .AsNoTracking()
                    .Select(x => new OrderCustomerDetailGetModel.ItemOrder
                    {
                        ItemId = x.Item.ItemId,
                        ItemSKU = x.Item.SKU,
                        Description = x.Item.Description,
                        UOM = x.Item.UnitOfMeasure.Code,
                        OrderQty = x.Qty,
                        QtyOnHand = x.Item.Inventory.QtyOnHand,
                        BinLocations = x.Item.Inventory == null ? 0 : x.Item.Inventory.InventoryBins.Count(),
                        PerUnitPrice = x.PerUnitPrice
                    })
                    .ToListAsync();

                var model = new OrderCustomerDetailGetModel();
                model.Orders = so.Count;
                model.UniqueItemsOrdered = ol.Count;
                model.SaleOrders = soh;
                model.CustomerDetails = so;
                model.ItemOrders = ol;

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