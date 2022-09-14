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
    public class SaleOrderService : ISaleOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public SaleOrderService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<SaleOrderGetModel>> CreateSaleOrderAsync(AppState state, SaleOrderCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(SaleOrderCreateModel.CustomerId)} is required.");
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

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(SaleOrderCreateModel.CustomerLocationId)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerLocations
                        .AnyAsync(x => x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == model.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                if (model.CustomerFacilityId is null)
                {
                    return Result.Fail($"{nameof(SaleOrderCreateModel.CustomerFacilityId)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerFacilities
                        .AnyAsync(x => x.CustomerFacilityId == model.CustomerFacilityId
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerFacility)} not found.");
                    }
                }

                Random random = new Random();
                var generatedSaleOrderNo = random.Next(0, 1000000).ToString("D6");

                 // SaleOrderNo uniqueness at customer level
                var saleOrderNoExist = await _context.SaleOrders
                                          .Include(x => x.CustomerLocation)
                                          .AnyAsync(x => x.SaleOrderNo.Trim().ToLower() == generatedSaleOrderNo.Trim().ToLower()
                                   && x.CustomerLocation.CustomerId == model.CustomerId);

                if (saleOrderNoExist)
                {
                    while(saleOrderNoExist)
                    {
                        generatedSaleOrderNo = random.Next(0, 1000000).ToString("D6");

                        // SaleOrderNo uniqueness at customer level
                        saleOrderNoExist = await _context.SaleOrders
                                                         .Include(x => x.CustomerLocation)
                                                         .AnyAsync(x => x.SaleOrderNo.Trim().ToLower() == generatedSaleOrderNo.Trim().ToLower()
                                                     && x.CustomerLocation.CustomerId == model.CustomerId);
                    }
                }

                if (model.OrderDate is null)
                {
                    return Result.Fail($"{nameof(SaleOrderCreateModel.OrderDate)} is required.");
                }

                if (model.OrderCustomerId is null)
                {
                    return Result.Fail($"{nameof(SaleOrderCreateModel.OrderCustomerId)} is required.");
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

                var shippingAddress = await _context.OrderCustomerAddresses.FirstAsync(x => x.AddressType == OrderCustomerAddressType.ShippingAddress
                                                                                         && x.OrderCustomerId == model.OrderCustomerId);

                var billingAddress = await _context.OrderCustomerAddresses.FirstAsync(x => x.AddressType == OrderCustomerAddressType.BillingAddress
                                                                                        && x.OrderCustomerId == model.OrderCustomerId);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = new SaleOrder
                        {
                            CustomerLocationId = model.CustomerLocationId,
                            CustomerFacilityId = model.CustomerFacilityId,
                            SaleOrderNo = generatedSaleOrderNo,
                            Status = SaleOrderStatusEnum.Pending,
                            OrderDate = model.OrderDate.Value,
                            PromiseDate = model.PromiseDate.Value,
                            OrderQty = 0,
                            Remaining = 0,
                            OrderCustomerId = model.OrderCustomerId,
                            ShippingAddressId = shippingAddress.OrderCustomerAddressId,
                            BillingAddressId = billingAddress.OrderCustomerAddressId,
                            PickingStatus = PickingStatusEnum.Pending
                        };

                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var customer = await _context.OrderCustomers
                            .SingleOrDefaultAsync(x => x.OrderCustomerId == model.OrderCustomerId);
                        customer.LastOrderDate = DateTime.Now;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        entity = await _context.SaleOrders
                            .Include(x => x.OrderCustomer)
                            .Include(x => x.ShippingAddress)
                            .Include(x => x.BillingAddress)
                            .SingleOrDefaultAsync(x => x.SaleOrderId == entity.SaleOrderId);

                        return Result.Ok(new SaleOrderGetModel
                        {
                            SaleOrderId = entity.SaleOrderId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            CustomerFacilityId = entity.CustomerFacilityId.Value,
                            SaleOrderNo = entity.SaleOrderNo,
                            Status = entity.Status,
                            OrderDate = entity.OrderDate,
                            PromiseDate = entity.PromiseDate,
                            FulfilledDate = entity.FulfilledDate,
                            OrderQty = entity.OrderQty,
                            TotalSalePrice = entity.TotalSalePrice,
                            OrderCustomerId = entity.OrderCustomer.OrderCustomerId,
                            CustomerName = entity.OrderCustomer.Name,
                            ShipToAddress1 = entity.ShippingAddress.Address1,
                            ShipToAddress2 = entity.ShippingAddress.Address2,
                            ShipToStateProvince = entity.ShippingAddress.StateProvince,
                            ShipToCity = entity.ShippingAddress.City,
                            ShipToZipPostalCode = entity.ShippingAddress.ZipPostalCode,
                            ShipToCountry = entity.ShippingAddress.Country,
                            ShipToPhoneNumber = entity.ShippingAddress.PhoneNumber,
                            BillToAddress1 = entity.BillingAddress.Address1,
                            BillToAddress2 = entity.BillingAddress.Address2,
                            BillToStateProvince = entity.BillingAddress.StateProvince,
                            BillToCity = entity.BillingAddress.City,
                            BillToZipPostalCode = entity.BillingAddress.ZipPostalCode,
                            BillToCountry = entity.BillingAddress.Country,
                            BillToPhoneNumber = entity.BillingAddress.PhoneNumber,
                            PickingStatus = entity.PickingStatus
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

        public async Task<Result<PurchaseOrderImportModel>> AddImportedSaleOrdersAsync(AppState state, int customerLocationId, SalesOrderImportModel[] model)
        {
            try
            {
                List<Receive> receives = new List<Receive>();
                SaleOrder currentSaleOrder = new SaleOrder();  // keeps track of what PO to reference when creating new Receives
                List<string> uniqueSONumbers = new List<string>();

                // Terminating early on duplicate PONumber for customer in system
                foreach (SalesOrderImportModel order in model)
                {
                    SaleOrder existingPO = await _context.SaleOrders.FirstOrDefaultAsync(so => so.SaleOrderNo == order.SaleOrderNo
                                                                                                    && so.CustomerLocationId == customerLocationId
                                                                                                    && !so.Deleted);
                    if (existingPO != null)
                    {
                        return Result.Fail($"A sales order with number {existingPO.SaleOrderNo} already exists. Import was terminated");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (SalesOrderImportModel saleOrder in model)
                        {
                            if (!uniqueSONumbers.Contains(saleOrder.SaleOrderNo))
                            {
                                uniqueSONumbers.Add(saleOrder.SaleOrderNo);

                                SaleOrder saleOrderEntity = new SaleOrder()
                                {
                                    CustomerLocationId = saleOrder.CustomerLocationId.Value,
                                    CustomerFacilityId = saleOrder.CustomerFacilityId.Value,
                                    SaleOrderNo = saleOrder.SaleOrderNo,
                                    Status = SaleOrderStatusEnum.Pending,
                                    OrderDate = saleOrder.OrderDate.Value,
                                    PromiseDate = saleOrder.PromiseDate,
                                    FulfilledDate = saleOrder.FulfilledDate,
                                    ShippingAddressId = 0,
                                    BillingAddressId = 0,
                                    OrderQty = 0,
                                    PickingStatus = PickingStatusEnum.Pending
                                };

                                if (saleOrderEntity.SaleOrderNo == null)
                                {
                                    Random random = new Random();
                                    var generatedSaleOrderNo = random.Next(0, 1000000).ToString("D6");

                                    // SaleOrderNo uniqueness at customer level
                                    var saleOrderNoExist = await _context.SaleOrders
                                                              .Include(x => x.CustomerLocation)
                                                              .AnyAsync(x => x.SaleOrderNo.Trim().ToLower() == generatedSaleOrderNo.Trim().ToLower()
                                                       && x.CustomerLocation.CustomerId == saleOrder.CustomerId);

                                    if (saleOrderNoExist)
                                    {
                                        while (saleOrderNoExist)
                                        {
                                            generatedSaleOrderNo = random.Next(0, 1000000).ToString("D6");

                                            // SaleOrderNo uniqueness at customer level
                                            saleOrderNoExist = await _context.SaleOrders
                                                                             .Include(x => x.CustomerLocation)
                                                                             .AnyAsync(x => x.SaleOrderNo.Trim().ToLower() == generatedSaleOrderNo.Trim().ToLower()
                                                                         && x.CustomerLocation.CustomerId == saleOrder.CustomerId);
                                        }
                                    }

                                    saleOrderEntity.SaleOrderNo = generatedSaleOrderNo;
                                }

                                var existingOrderCustomer = await _context.OrderCustomers
                                                             .Where(c => c.Name.Trim().ToLower().Contains(saleOrder.CustomerName)
                                                                 || c.Name.Trim().ToLower() == saleOrder.CustomerName.Trim().ToLower())
                                                             .FirstOrDefaultAsync();

                                if (existingOrderCustomer != null)
                                {
                                    var shippingAddressExists = await _context.OrderCustomerAddresses.FirstOrDefaultAsync(x => x.AddressType == OrderCustomerAddressType.ShippingAddress
                                                                                         && x.OrderCustomerId == existingOrderCustomer.OrderCustomerId);

                                    var billingAddressExists = await _context.OrderCustomerAddresses.FirstOrDefaultAsync(x => x.AddressType == OrderCustomerAddressType.BillingAddress
                                                                                         && x.OrderCustomerId == existingOrderCustomer.OrderCustomerId);

                                    if (shippingAddressExists != null)
                                    {
                                        saleOrderEntity.ShippingAddressId = shippingAddressExists.OrderCustomerAddressId;
                                    }
                                    else
                                    {
                                        if (saleOrder.ShippingAddress1 == null)
                                        {
                                            return Result.Fail($"No shipping address data provided for {saleOrder.CustomerName}, and no addresses on record.");
                                        }
                                        else
                                        {
                                            var customerShippingAddressEntity = new OrderCustomerAddress
                                            {
                                                AddressType = OrderCustomerAddressType.ShippingAddress,
                                                OrderCustomerId = existingOrderCustomer.OrderCustomerId,
                                                Address1 = saleOrder.ShippingAddress1,
                                                City = saleOrder.ShippingCity,
                                                Address2 = saleOrder.ShippingAddress2,
                                                StateProvince = saleOrder.ShippingZipPostalCode,
                                                ZipPostalCode = saleOrder.ShippingZipPostalCode,
                                                Country = saleOrder.ShippingCountry,
                                                PhoneNumber = saleOrder.ShippingPhoneNumber,
                                            };

                                            await _context.OrderCustomerAddresses.AddAsync(customerShippingAddressEntity);
                                        }
                                    }
                                    if (billingAddressExists != null)
                                    {
                                        saleOrderEntity.BillingAddressId = billingAddressExists.OrderCustomerAddressId;
                                    }
                                    else
                                    {
                                        if (saleOrder.BillingAddress1 == null)
                                        {
                                            return Result.Fail($"No billing address data provided for {saleOrder.CustomerName}, and no addresses on record.");
                                        }
                                        else
                                        {
                                            var customerBillingAddressEntity = new OrderCustomerAddress
                                            {
                                                AddressType = OrderCustomerAddressType.BillingAddress,
                                                OrderCustomerId = existingOrderCustomer.OrderCustomerId,
                                                Address1 = saleOrder.BillingAddress1,
                                                City = saleOrder.BillingCity,
                                                Address2 = saleOrder.BillingAddress2,
                                                StateProvince = saleOrder.BillingZipPostalCode,
                                                ZipPostalCode = saleOrder.BillingZipPostalCode,
                                                Country = saleOrder.BillingCountry,
                                                PhoneNumber = saleOrder.BillingPhoneNumber,
                                            };

                                            await _context.OrderCustomerAddresses.AddAsync(customerBillingAddressEntity);
                                        }
                                    }

                                    saleOrderEntity.OrderCustomerId = existingOrderCustomer.OrderCustomerId;
                                }
                                else if (existingOrderCustomer == null)
                                {
                                    var orderCustomerEntity = new OrderCustomer
                                    {
                                        CustomerId = saleOrder.CustomerId,
                                        Name = saleOrder.CustomerName,
                                        PaymentType = OrderCustomerPaymentTypeEnum.SalesOrder,
                                    };

                                    await _context.OrderCustomers.AddAsync(orderCustomerEntity);
                                    await _context.SaveChangesAsync();

                                    var newOrderCustomer = await _context.OrderCustomers.FirstOrDefaultAsync(c => c.Name.Trim().ToLower() == saleOrder.CustomerName.Trim().ToLower());

                                    saleOrderEntity.OrderCustomerId = newOrderCustomer.OrderCustomerId;

                                    var customerShippingAddressEntity = new OrderCustomerAddress
                                    {
                                        AddressType = OrderCustomerAddressType.ShippingAddress,
                                        OrderCustomerId = newOrderCustomer.OrderCustomerId,
                                        Address1 = saleOrder.ShippingAddress1,
                                        City = saleOrder.ShippingCity,
                                        Address2 = saleOrder.ShippingAddress2,
                                        StateProvince = saleOrder.ShippingStateProvince,
                                        ZipPostalCode = saleOrder.ShippingZipPostalCode,
                                        Country = saleOrder.ShippingCountry,
                                        PhoneNumber = saleOrder.ShippingPhoneNumber,
                                    };

                                    var customerBillingAddressEntity = new OrderCustomerAddress
                                    {
                                        AddressType = OrderCustomerAddressType.BillingAddress,
                                        OrderCustomerId = newOrderCustomer.OrderCustomerId,
                                        Address1 = saleOrder.BillingAddress1,
                                        City = saleOrder.BillingCity,
                                        Address2 = saleOrder.BillingAddress2,
                                        StateProvince = saleOrder.BillingStateProvince,
                                        ZipPostalCode = saleOrder.BillingZipPostalCode,
                                        Country = saleOrder.BillingCountry,
                                        PhoneNumber = saleOrder.BillingPhoneNumber,
                                    };

                                    if (customerBillingAddressEntity.Address1 == null)
                                    {
                                        customerBillingAddressEntity = customerShippingAddressEntity;
                                    }


                                    await _context.OrderCustomerAddresses.AddAsync(customerShippingAddressEntity);
                                    await _context.OrderCustomerAddresses.AddAsync(customerBillingAddressEntity);
                                    await _context.SaveChangesAsync();

                                    List<OrderCustomerAddress> addresses = await _context.OrderCustomerAddresses.Where(a => a.OrderCustomerId == newOrderCustomer.OrderCustomerId
                                                    ).ToListAsync();

                                    saleOrderEntity.ShippingAddressId = addresses.Where(a => a.AddressType == OrderCustomerAddressType.ShippingAddress)
                                                                                 .FirstOrDefault().OrderCustomerAddressId;
                                    saleOrderEntity.BillingAddressId = addresses.Where(a => a.AddressType == OrderCustomerAddressType.BillingAddress)
                                                                                 .FirstOrDefault().OrderCustomerAddressId;
                                }

                                await _context.SaleOrders.AddAsync(saleOrderEntity);
                                await _context.SaveChangesAsync();

                                currentSaleOrder = saleOrderEntity;

                            }
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ex = await _exceptionService.HandleExceptionAsync(ex);
                        return Result.Fail(ex.ToString());
                    }
                }

                return Result.Ok();

            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<SaleOrderAllGetModel>> GetSaleOrderAllAsync(AppState state, int customerLocationId, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var so = await _context.SaleOrders
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                    .Include(x => x.OrderCustomer)
                    .Include(x => x.ShippingAddress)
                    .Include(x => x.BillingAddress)
                    .AsNoTracking()
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId)
                    .ToListAsync();

                var model = new SaleOrderAllGetModel();
                model.PendingOrders = so.Where(x => x.Status == SaleOrderStatusEnum.Pending).Count();
                model.InPicking = so.Where(x => x.Status == SaleOrderStatusEnum.Printed).Count();

                var dto = new List<SaleOrderAllGetModel.SaleOrder>();
                foreach (var x in so.OrderByDescending(x => x.OrderDate))
                {
                    var z = new SaleOrderAllGetModel.SaleOrder();
                    z.SaleOrderId = x.SaleOrderId;
                    z.Status = x.Status.ToLabel();
                    z.OrderNo = x.SaleOrderNo;
                    z.OrderDate = x.OrderDate;
                    z.PromiseDate = x.PromiseDate;
                    z.FulfilledDate = x.FulfilledDate;
                    z.OrderCustomerId = x.OrderCustomer.OrderCustomerId;
                    z.CustomerName = x.OrderCustomer.Name;

                    if (x.OrderLines.Count == 1)
                    {
                        z.ItemId = x.OrderLines.FirstOrDefault().Item.ItemId;
                        z.ItemSKU = x.OrderLines.FirstOrDefault().Item.SKU;
                        z.QtyOnHand = x.OrderLines.FirstOrDefault().Item.Inventory.QtyOnHand.ToString();
                    }
                    else if (x.OrderLines.Count > 1)
                    {
                        z.ItemId = null;
                        z.ItemSKU = x.OrderLines.Count.ToString();
                        z.QtyOnHand = "N/A";
                    }

                    z.QtyOrdered = x.OrderQty;
                    z.TotalSalePrice = x.TotalSalePrice;

                    dto.Add(z);
                }
                model.SaleOrders = dto;

                if (model is null)
                {
                    return Result.Fail($"{nameof(SaleOrder)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<SaleOrderDetailGetModel>> GetSaleOrderDetailAsync(AppState state, int saleOrderId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var so = await _context.SaleOrders
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryZones)
                                    .ThenInclude(x => x.Zone)
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryBins)
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.OrderCustomer)
                    .Include(x => x.ShippingAddress)
                    .Include(x => x.BillingAddress)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.SaleOrderId == saleOrderId);

                if (so is null)
                {
                    return Result.Fail($"{nameof(SaleOrder)} not found.");
                }

                var model = new SaleOrderDetailGetModel();
                model.OrderDetails = new SaleOrderDetailGetModel.OrderDetail();
                model.OrderDetails.SaleOrderId = so.SaleOrderId;
                model.OrderDetails.OrderNo = so.SaleOrderNo;
                model.OrderDetails.OrderDate = so.OrderDate;
                model.OrderDetails.PromiseDate = so.PromiseDate;
                model.OrderDetails.FulfilledDate = so.FulfilledDate;
                model.OrderDetails.OrderCustomerId = so.OrderCustomer.OrderCustomerId;
                model.OrderDetails.CustomerName = so.OrderCustomer.Name;
                model.OrderDetails.QtyOrdered = so.OrderQty;
                model.OrderDetails.TotalSalePrice = so.TotalSalePrice;
                model.OrderDetails.ShipToAddress1 = so.ShippingAddress.Address1;
                model.OrderDetails.ShipToAddress2 = so.ShippingAddress.Address2;
                model.OrderDetails.ShipToCity = so.ShippingAddress.City;
                model.OrderDetails.ShipToStateProvince = so.ShippingAddress.StateProvince;
                model.OrderDetails.ShipToZipPostalCode = so.ShippingAddress.ZipPostalCode;
                model.OrderDetails.ShipToCountry = so.ShippingAddress.Country;
                model.OrderDetails.ShipToPhoneNumber = so.ShippingAddress.PhoneNumber;
                model.OrderDetails.BillToAddress1 = so.BillingAddress.Address1;
                model.OrderDetails.BillToAddress2 = so.BillingAddress.Address2;
                model.OrderDetails.BillToCity = so.BillingAddress.City;
                model.OrderDetails.BillToStateProvince = so.BillingAddress.StateProvince;
                model.OrderDetails.BillToZipPostalCode = so.BillingAddress.ZipPostalCode;
                model.OrderDetails.BillToCountry = so.BillingAddress.Country;
                model.OrderDetails.BillToPhoneNumber = so.BillingAddress.PhoneNumber;

                if (so.OrderLines.Count == 1)
                {
                    model.OrderDetails.ItemId = so.OrderLines.FirstOrDefault().Item.ItemId;
                    model.OrderDetails.ItemSKU = so.OrderLines.FirstOrDefault().Item.SKU;
                    model.OrderDetails.QtyOnHand = so.OrderLines.FirstOrDefault().Item.Inventory.QtyOnHand.ToString();
                }
                else if (so.OrderLines.Count > 1)
                {
                    model.OrderDetails.ItemId = null;
                    model.OrderDetails.ItemSKU = so.OrderLines.Count.ToString();
                    model.OrderDetails.QtyOnHand = "N/A";
                }

                var dto = new List<SaleOrderDetailGetModel.OrderLine>();
                foreach (var x in so.OrderLines)
                {
                    var z = new SaleOrderDetailGetModel.OrderLine();
                    z.OrderLineId = x.OrderLineId;
                    z.ItemId = x.Item.ItemId;
                    z.ItemSKU = x.Item.SKU;
                    z.Description = x.Item.Description;
                    z.UOM = x.Item.UnitOfMeasure.Code;
                    z.OrderQty = x.Qty;
                    z.PerUnitPrice = x.PerUnitPrice;

                    if (x.Item.Inventory.InventoryZones.Count == 1)
                    {
                        z.ZoneId = x.Item.Inventory.InventoryZones.FirstOrDefault().Zone.ZoneId;
                        z.Zone = x.Item.Inventory.InventoryZones.FirstOrDefault().Zone.Name;
                    }
                    else if (x.Item.Inventory.InventoryZones.Count > 1)
                    {
                        z.ZoneId = null;
                        z.Zone = x.Item.Inventory.InventoryZones.Count.ToString();
                    }

                    z.BinLocations = x.Item.Inventory.InventoryBins.Count;

                    dto.Add(z);
                }
                model.OrderLines = dto;

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<SaleOrderGetModel>> UpdateSaleOrderStatusToPrintedAsync(AppState state, int saleOrderId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var entity = await _context.SaleOrders
                    .Include(x => x.OrderCustomer)
                    .Include(x => x.ShippingAddress)
                    .Include(x => x.BillingAddress)
                    .SingleOrDefaultAsync(x => x.SaleOrderId == saleOrderId
                        && x.Status == SaleOrderStatusEnum.Pending);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(SaleOrder)} not found.");
                }

                if (entity.OrderQty == 0)
                {
                    return Result.Fail("Cannot change the status to printed because this sale order doesn't have any order line.");
                }

                entity.Status = SaleOrderStatusEnum.Printed;

                await _context.SaveChangesAsync();

                return Result.Ok(new SaleOrderGetModel
                {
                    SaleOrderId = entity.SaleOrderId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    CustomerFacilityId = entity.CustomerFacilityId.Value,
                    SaleOrderNo = entity.SaleOrderNo,
                    Status = entity.Status,
                    OrderDate = entity.OrderDate,
                    PromiseDate = entity.PromiseDate,
                    FulfilledDate = entity.FulfilledDate,
                    OrderQty = entity.OrderQty,
                    TotalSalePrice = entity.TotalSalePrice,
                    OrderCustomerId = entity.OrderCustomer.OrderCustomerId,
                    CustomerName = entity.OrderCustomer.Name,
                    ShipToAddress1 = entity.ShippingAddress.Address1,
                    ShipToAddress2 = entity.ShippingAddress.Address2,
                    ShipToStateProvince = entity.ShippingAddress.StateProvince,
                    ShipToZipPostalCode = entity.ShippingAddress.ZipPostalCode,
                    ShipToCountry = entity.ShippingAddress.Country,
                    ShipToCity = entity.ShippingAddress.City,
                    ShipToPhoneNumber = entity.ShippingAddress.PhoneNumber,
                    BillToAddress1 = entity.BillingAddress.Address1,
                    BillToAddress2 = entity.BillingAddress.Address2,
                    BillToStateProvince = entity.BillingAddress.StateProvince,
                    BillToZipPostalCode = entity.BillingAddress.ZipPostalCode,
                    BillToCountry = entity.BillingAddress.Country,
                    BillToCity = entity.BillingAddress.City,
                    BillToPhoneNumber = entity.BillingAddress.PhoneNumber,
                    PickingStatus = entity.PickingStatus
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<SaleOrderPrintGetModel>> GetSaleOrderPrintAsync(AppState state, int saleOrderId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var so = await _context.SaleOrders
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryZones)
                                    .ThenInclude(x => x.Zone)
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryBins)
                                    .ThenInclude(x => x.Bin)
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.OrderCustomer)
                    .Include(x => x.ShippingAddress)
                    .Include(x => x.BillingAddress)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.SaleOrderId == saleOrderId);

                if (so is null)
                {
                    return Result.Fail($"{nameof(SaleOrder)} not found.");
                }

                var model = new SaleOrderPrintGetModel();
                model.OrderDetail = new SaleOrderPrintGetModel.SaleOrder();
                model.OrderDetail.SaleOrderNo = so.SaleOrderNo;
                model.OrderDetail.OrderDate = so.OrderDate;
                model.OrderDetail.PromiseDate = so.PromiseDate;
                model.OrderDetail.FulfilledDate = so.FulfilledDate;
                model.OrderDetail.OrderQty = so.OrderQty;
                model.OrderDetail.TotalSalePrice = so.TotalSalePrice;
                model.OrderDetail.OrderCustomerId = so.OrderCustomer.OrderCustomerId;
                model.OrderDetail.CustomerName = so.OrderCustomer.Name;
                model.OrderDetail.ShipToAddress1 = so.ShippingAddress.Address1;
                model.OrderDetail.ShipToAddress2 = so.ShippingAddress.Address2;
                model.OrderDetail.ShipToStateProvince = so.ShippingAddress.StateProvince;
                model.OrderDetail.ShipToZipPostalCode = so.ShippingAddress.ZipPostalCode;
                model.OrderDetail.ShipToCountry = so.ShippingAddress.Country;
                model.OrderDetail.ShipToCity = so.ShippingAddress.City;
                model.OrderDetail.ShipToPhoneNumber = so.ShippingAddress.PhoneNumber;
                model.OrderDetail.BillToAddress1 = so.BillingAddress.Address1;
                model.OrderDetail.BillToAddress2 = so.BillingAddress.Address2;
                model.OrderDetail.BillToStateProvince = so.BillingAddress.StateProvince;
                model.OrderDetail.BillToZipPostalCode = so.BillingAddress.ZipPostalCode;
                model.OrderDetail.BillToCountry = so.BillingAddress.Country;
                model.OrderDetail.BillToCity = so.BillingAddress.City;
                model.OrderDetail.BillToPhoneNumber = so.BillingAddress.PhoneNumber;

                model.OrderLines = so.OrderLines.Select(x => new SaleOrderPrintGetModel.OrderLine
                {
                    OrderLineId = x.OrderLineId,
                    ItemId = x.Item.ItemId,
                    ItemSKU = x.Item.SKU,
                    Description = x.Item.Description,
                    UOM = x.Item.UnitOfMeasure.Code,
                    OrderQty = x.Qty,
                    Zones = x.Item.Inventory.InventoryZones.Select(z => new SaleOrderPrintGetModel.Zone
                    {
                        ZoneId = z.Zone.ZoneId,
                        Name = z.Zone.Name,
                        Bins = z.Inventory.InventoryBins.Select(xx => new SaleOrderPrintGetModel.Bin
                        {
                            BinId = xx.Bin.BinId,
                            Name = xx.Bin.Name
                        })
                    })
                });

                return Result.Ok(model);

            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<List<SaleOrderPrintGetModel>>> GetSaleOrderPrintMultipleAsync(AppState state, int[] saleOrderIds)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                List<SaleOrderPrintGetModel> saleOrdersToPrint = new List<SaleOrderPrintGetModel>();

                foreach (int soId in saleOrderIds)
                {
                    var so = await _context.SaleOrders
                        .Include(x => x.OrderLines)
                            .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.Inventory)
                                    .ThenInclude(x => x.InventoryZones)
                                        .ThenInclude(x => x.Zone)
                        .Include(x => x.OrderLines)
                            .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.Inventory)
                                    .ThenInclude(x => x.InventoryBins)
                                        .ThenInclude(x => x.Bin)
                        .Include(x => x.OrderLines)
                            .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.UnitOfMeasure)
                        .Include(x => x.OrderCustomer)
                        .Include(x => x.ShippingAddress)
                        .Include(x => x.BillingAddress)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.SaleOrderId == soId);

                    if (so is null)
                    {
                        return Result.Fail($"{nameof(SaleOrder)} not found.");
                    }

                    var model = new SaleOrderPrintGetModel();
                    model.OrderDetail = new SaleOrderPrintGetModel.SaleOrder();
                    model.OrderDetail.SaleOrderId = so.SaleOrderId;
                    model.OrderDetail.SaleOrderNo = so.SaleOrderNo;
                    model.OrderDetail.OrderDate = so.OrderDate;
                    model.OrderDetail.PromiseDate = so.PromiseDate;
                    model.OrderDetail.FulfilledDate = so.FulfilledDate;
                    model.OrderDetail.OrderQty = so.OrderQty;
                    model.OrderDetail.TotalSalePrice = so.TotalSalePrice;
                    model.OrderDetail.OrderCustomerId = so.OrderCustomer.OrderCustomerId;
                    model.OrderDetail.CustomerName = so.OrderCustomer.Name;
                    model.OrderDetail.ShipToAddress1 = so.ShippingAddress.Address1;
                    model.OrderDetail.ShipToAddress2 = so.ShippingAddress.Address2;
                    model.OrderDetail.ShipToStateProvince = so.ShippingAddress.StateProvince;
                    model.OrderDetail.ShipToZipPostalCode = so.ShippingAddress.ZipPostalCode;
                    model.OrderDetail.ShipToCountry = so.ShippingAddress.Country;
                    model.OrderDetail.ShipToCity = so.ShippingAddress.City;
                    model.OrderDetail.ShipToPhoneNumber = so.ShippingAddress.PhoneNumber;
                    model.OrderDetail.BillToAddress1 = so.BillingAddress.Address1;
                    model.OrderDetail.BillToAddress2 = so.BillingAddress.Address2;
                    model.OrderDetail.BillToStateProvince = so.BillingAddress.StateProvince;
                    model.OrderDetail.BillToZipPostalCode = so.BillingAddress.ZipPostalCode;
                    model.OrderDetail.BillToCountry = so.BillingAddress.Country;
                    model.OrderDetail.BillToCity = so.BillingAddress.City;
                    model.OrderDetail.BillToPhoneNumber = so.BillingAddress.PhoneNumber;

                    model.OrderLines = so.OrderLines.Select(x => new SaleOrderPrintGetModel.OrderLine
                    {
                        OrderLineId = x.OrderLineId,
                        ItemId = x.Item.ItemId,
                        ItemSKU = x.Item.SKU,
                        Description = x.Item.Description,
                        UOM = x.Item.UnitOfMeasure.Code,
                        OrderQty = x.Qty,
                        Zones = x.Item.Inventory.InventoryZones.Select(z => new SaleOrderPrintGetModel.Zone
                        {
                            ZoneId = z.Zone.ZoneId,
                            Name = z.Zone.Name,
                            Bins = z.Inventory.InventoryBins.Select(xx => new SaleOrderPrintGetModel.Bin
                            {
                                BinId = xx.Bin.BinId,
                                Name = xx.Bin.Name
                            })
                        })
                    });

                    saleOrdersToPrint.Add(model);
                }


                return Result.Ok(saleOrdersToPrint);

            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<SaleOrderPickQueueGetModel>> GetSaleOrderPickQueueAsync(AppState state, int customerLocationId, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                IEnumerable<SaleOrderPickQueueGetModel.Item> so = await _context.SaleOrders
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryBins)
                    .AsNoTracking()
                    .Where(x => x.Status == SaleOrderStatusEnum.Printed
                        && x.PickingStatus != PickingStatusEnum.Complete
                        && x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId)
                    .Select(x => new SaleOrderPickQueueGetModel.Item
                    {
                        SaleOrderId = x.SaleOrderId,
                        SaleOrderNo = x.SaleOrderNo,
                        PickingStatus = x.PickingStatus.ToLabel(),
                        Units = x.OrderLines.Sum(z => z.Qty),
                        Locations = x.OrderLines.Select(z => z.Item.Inventory.InventoryBins.Count()).Sum()
                    })
                    .ToListAsync();

                var model = new SaleOrderPickQueueGetModel();
                model.ItemCount = await _context.SaleOrders
                    .AsNoTracking()
                    .Where(x => x.Status == SaleOrderStatusEnum.Printed
                        && x.PickingStatus != PickingStatusEnum.Complete
                        && x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId)
                    .Select(x => x.OrderLines.Count)
                    .CountAsync();
                model.CompletedToday = await _context.SaleOrders
                    .AsNoTracking()
                    .Where(x => x.Status == SaleOrderStatusEnum.Printed
                        && x.PickingStatus == PickingStatusEnum.Complete
                        && x.FulfilledDate.Value.Date == DateTime.Now.Date
                        && x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId)
                    .CountAsync();
                model.UnitsPending = await _context.SaleOrders
                    .AsNoTracking()
                    .Where(x => x.Status == SaleOrderStatusEnum.Printed
                        && x.PickingStatus == PickingStatusEnum.Pending
                        && x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId)
                    .CountAsync();
                model.Items = so;

                if (model is null)
                {
                    return Result.Fail($"{nameof(SaleOrder)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>>> GetSaleOrderPickQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (barcodeSearch && !searchText.HasValue())
                {
                    return Result.Fail("Order no is required.");
                }

                var query = _context.SaleOrders
                    .Include(x => x.OrderLines)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryBins)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (barcodeSearch)
                    {
                        query = query.Where(x => x.Status == SaleOrderStatusEnum.Printed
                            && x.PickingStatus != PickingStatusEnum.Complete
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == customerFacilityId
                            && x.SaleOrderNo.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => x.Status == SaleOrderStatusEnum.Printed
                            && x.PickingStatus != PickingStatusEnum.Complete
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == customerFacilityId
                            && x.SaleOrderNo.Trim().ToLower().Contains(searchText));
                    }
                }
                else
                {
                    query = query.Where(x => x.Status == SaleOrderStatusEnum.Printed
                        && x.PickingStatus != PickingStatusEnum.Complete
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerFacilityId == customerFacilityId);
                }

                IEnumerable<SaleOrderPickQueueLookupDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new SaleOrderPickQueueLookupDeviceGetModel
                    {
                        SaleOrderId = x.SaleOrderId,
                        SaleOrderNo = x.SaleOrderNo,
                        PickingStatus = x.PickingStatus,
                        Items = x.OrderLines.Count,
                        Bins = x.OrderLines.Select(z => z.Item.Inventory.InventoryBins.Count()).Sum()
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(SaleOrder)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<SaleOrderGetModel>> UpdateSaleOrderPickingStatusDeviceAsync(CustomerDeviceTokenAuthModel state, SaleOrderPickingStatusUpdateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(SaleOrderPickingStatusUpdateModel)} is required.");
                }

                if (model.SaleOrderId is null)
                {
                    return Result.Fail($"{nameof(SaleOrderPickingStatusUpdateModel.SaleOrderId)} is required.");
                }
                else
                {
                    var exist = await _context.SaleOrders
                        .AnyAsync(x => x.SaleOrderId == model.SaleOrderId
                            && x.CustomerLocationId == state.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(SaleOrder)} not found.");
                    }
                }

                if (model.PickingStatus is null)
                {
                    return Result.Fail($"{nameof(SaleOrderPickingStatusUpdateModel.PickingStatus)} is required.");
                }

                var entity = await _context.SaleOrders
                    .Include(x => x.OrderCustomer)
                    .Include(x => x.ShippingAddress)
                    .Include(x => x.BillingAddress)
                    .SingleOrDefaultAsync(x => x.SaleOrderId == model.SaleOrderId
                            && x.CustomerLocationId == state.CustomerLocationId);

                entity.PickingStatus = model.PickingStatus.Value;

                if (model.PickingStatus == PickingStatusEnum.Complete)
                {
                    entity.FulfilledDate = DateTime.Now;
                }
                else
                {
                    entity.FulfilledDate = null;
                }

                await _context.SaveChangesAsync();

                return Result.Ok(new SaleOrderGetModel
                {
                    SaleOrderId = entity.SaleOrderId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    CustomerFacilityId = entity.CustomerFacilityId.Value,
                    SaleOrderNo = entity.SaleOrderNo,
                    Status = entity.Status,
                    OrderDate = entity.OrderDate,
                    PromiseDate = entity.PromiseDate,
                    FulfilledDate = entity.FulfilledDate,
                    OrderQty = entity.OrderQty,
                    TotalSalePrice = entity.TotalSalePrice,
                    OrderCustomerId = entity.OrderCustomer.OrderCustomerId,
                    CustomerName = entity.OrderCustomer.Name,
                    ShipToAddress1 = entity.ShippingAddress.Address1,
                    ShipToAddress2 = entity.ShippingAddress.Address2,
                    ShipToStateProvince = entity.ShippingAddress.StateProvince,
                    ShipToZipPostalCode = entity.ShippingAddress.ZipPostalCode,
                    ShipToCountry = entity.ShippingAddress.Country,
                    ShipToCity = entity.ShippingAddress.City,
                    ShipToPhoneNumber = entity.ShippingAddress.PhoneNumber,
                    BillToAddress1 = entity.BillingAddress.Address1,
                    BillToAddress2 = entity.BillingAddress.Address2,
                    BillToStateProvince = entity.BillingAddress.StateProvince,
                    BillToZipPostalCode = entity.BillingAddress.ZipPostalCode,
                    BillToCountry = entity.BillingAddress.Country,
                    BillToCity = entity.BillingAddress.City,
                    BillToPhoneNumber = entity.BillingAddress.PhoneNumber,
                    PickingStatus = entity.PickingStatus
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }
    }
}