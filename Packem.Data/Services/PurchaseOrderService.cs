using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Entities;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public PurchaseOrderService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<PurchaseOrderGetModel>> CreatePurchaseOrderAsync(AppState state, PurchaseOrderCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrderCreateModel.CustomerLocationId)} is required.");
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

                if (model.CustomerFacilityId is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrderCreateModel.CustomerFacilityId)} is required.");
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

                if (!model.PurchaseOrderNo.HasValue())
                {
                    return Result.Fail($"{nameof(PurchaseOrderCreateModel.PurchaseOrderNo)} is required.");
                }
                else
                {
                    var exist = await _context.PurchaseOrders
                        .AnyAsync(x => x.PurchaseOrderNo.Trim().ToLower() == model.PurchaseOrderNo.Trim().ToLower()
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(PurchaseOrderCreateModel.PurchaseOrderNo)} is already exist.");
                    }
                }

                if (!model.ShipVia.HasValue())
                {
                    return Result.Fail($"{nameof(PurchaseOrderCreateModel.ShipVia)} is required.");
                }

                if (model.VendorId is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrderCreateModel.VendorId)} is required.");
                }
                else
                {
                    var exist = await _context.Vendors
                        .Include(x => x.Customer)
                            .ThenInclude(x => x.CustomerLocations)
                        .AnyAsync(x => x.VendorId == model.VendorId
                            && x.Customer.CustomerLocations.Any(z => z.CustomerLocationId == model.CustomerLocationId));

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Vendor)} not found.");
                    }
                }

                if (model.OrderDate is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrderCreateModel.OrderDate)} is required.");
                }

                //if (model.OrderQty is null)
                //{
                //    return Result.Fail($"{nameof(PurchaseOrderCreateModel.OrderQty)} is required.");
                //}
                //else
                //{
                //    if (model.OrderQty.Value.IsNegative())
                //    {
                //        return Result.Fail($"{nameof(PurchaseOrderCreateModel.OrderQty)} cannot be negative.");
                //    }
                //    else if (model.OrderQty.Value.IsZero())
                //    {
                //        return Result.Fail($"{nameof(PurchaseOrderCreateModel.OrderQty)} cannot be zero.");
                //    }
                //}

                //var itemQtyOnHand = await _context.Items
                //    .Select(x => new
                //    {
                //        x.ItemId,
                //        x.QtyOnHand
                //    })
                //    .SingleOrDefaultAsync(x => x.ItemId == model.ItemId);

                //if (model.OrderQty > itemQtyOnHand.QtyOnHand)
                //{
                //    return Result.Fail($"Item only have a remaining quantity of: {itemQtyOnHand.QtyOnHand}. Cannot order more than the item quantity.");
                //}

                var entity = new PurchaseOrder
                {
                    CustomerLocationId = model.CustomerLocationId,
                    CustomerFacilityId = model.CustomerFacilityId,
                    VendorId = model.VendorId,
                    PurchaseOrderNo = model.PurchaseOrderNo,
                    Status = PurchaseOrderStatusEnum.NotReceived,
                    ShipVia = model.ShipVia,
                    OrderDate = model.OrderDate.Value,
                    OrderQty = 0,
                    Remaining = 0,
                    UpdatedDateTime = DateTime.UtcNow,
                    StatusUpdatedDateTime = DateTime.UtcNow,
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new PurchaseOrderGetModel
                {
                    PurchaseOrderId = entity.PurchaseOrderId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    CustomerFacilityId = entity.CustomerFacilityId.Value,
                    VendorId = entity.VendorId.Value,
                    PurchaseOrderNo = entity.PurchaseOrderNo,
                    Status = entity.Status,
                    OrderDate = entity.OrderDate,
                    OrderQty = entity.OrderQty,
                    Remaining = entity.Remaining,
                    UpdatedDateTime = entity.UpdatedDateTime
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<PurchaseOrderImportModel>> AddImportedPurchaseOrdersAsync(AppState state, int customerLocationId, PurchaseOrderImportModel[] model)
        {
            try
            {
                List<Receive> receives = new List<Receive>();
                PurchaseOrder currentPurchaseOrder = new PurchaseOrder();  // keeps track of what PO to reference when creating new Receives
                List<string> uniquePONumbers = new List<string>();

                // Terminating early on duplicate PONumber for customer in system
                foreach (PurchaseOrderImportModel order in model)
                {
                    PurchaseOrder existingPO = await _context.PurchaseOrders.FirstOrDefaultAsync(po => po.PurchaseOrderNo == order.PurchaseOrderNo
                                                                                                    && po.CustomerLocationId == customerLocationId
                                                                                                    && !po.Deleted);
                    if (existingPO != null)
                    {
                        return Result.Fail($"A purchase order with number {existingPO.PurchaseOrderNo} already exists. Import was terminated");
                    }

                    if (!String.IsNullOrEmpty(order.ItemSKU))
                    {
                        if (String.IsNullOrEmpty(order.ItemDescription))
                        {
                            return Result.Fail($"The item with SKU {order.ItemSKU} must have an item description. Import was terminated");
                        }

                        if (order.ItemUoM is null)
                        {
                            return Result.Fail($"The item with SKU {order.ItemSKU} must have an item UoM. Import was terminated");
                        }
                    }
                }

                foreach (PurchaseOrderImportModel purchaseOrder in model)
                {
                    if (!uniquePONumbers.Contains(purchaseOrder.PurchaseOrderNo))
                    {
                        uniquePONumbers.Add(purchaseOrder.PurchaseOrderNo);

                        Vendor existingVendor = _context.Vendors.FirstOrDefault(v => (v.VendorNo == purchaseOrder.VendorAccount || v.Name == purchaseOrder.VendorName)
                                                                            && v.CustomerId == state.CustomerId.Value
                                                                            && !v.Deleted);
                        Vendor vendorEntity = new Vendor();

                        if (existingVendor is null && (purchaseOrder.VendorName.ToLower() == "unknown" || String.IsNullOrEmpty(purchaseOrder.VendorAccount)))
                        {
                            // check to see if this customer already has an Unknown Vendor profile if not create one for them
                            Vendor unknownVendor = _context.Vendors.FirstOrDefault(v => v.Name.ToLower() == "unknown");

                            if (unknownVendor == null)
                            {
                                vendorEntity.CustomerId = state.CustomerId.Value;
                                vendorEntity.Name = "Unknown";
                                vendorEntity.VendorNo = "";
                                vendorEntity.Address1 = "";
                                vendorEntity.Address2 = "";
                                vendorEntity.PhoneNumber = "";
                                vendorEntity.PointOfContact = "";
                                vendorEntity.City = "";
                                vendorEntity.StateProvince = "";
                                vendorEntity.ZipPostalCode = "";

                                await _context.Vendors.AddAsync(vendorEntity);
                                await _context.SaveChangesAsync();

                            }
                            else
                            {
                                vendorEntity = unknownVendor;
                            }
                        }

                        if (existingVendor is null && !String.IsNullOrEmpty(purchaseOrder.VendorAccount))
                        {

                            vendorEntity.CustomerId = state.CustomerId.Value;
                            vendorEntity.Name = purchaseOrder.VendorName;
                            vendorEntity.VendorNo = purchaseOrder.VendorAccount;
                            vendorEntity.Address1 = purchaseOrder.VendorAddress;
                            vendorEntity.Address2 = purchaseOrder.VendorAddress2;
                            vendorEntity.PhoneNumber = purchaseOrder.VendorPhone;
                            vendorEntity.PointOfContact = purchaseOrder.PointOfCOntact;
                            vendorEntity.City = purchaseOrder.VendorCity;
                            vendorEntity.StateProvince = purchaseOrder.VendorStateOrProvince;
                            vendorEntity.ZipPostalCode = purchaseOrder.VendorZip;

                            await _context.Vendors.AddAsync(vendorEntity);
                            await _context.SaveChangesAsync();
                        }

                        if (existingVendor != null)
                        {
                            vendorEntity = existingVendor;
                        }

                        PurchaseOrder purchaseOrderEntity = new PurchaseOrder()
                        {
                            CustomerLocationId = purchaseOrder.CustomerLocationId.Value,
                            CustomerFacilityId = purchaseOrder.CustomerFacilityId.Value,
                            PurchaseOrderNo = purchaseOrder.PurchaseOrderNo,
                            VendorId = vendorEntity.VendorId,
                            ShipVia = purchaseOrder.ShipVia,
                            OrderDate = (DateTime)purchaseOrder.OrderDate,
                            Status = PurchaseOrderStatusEnum.NotReceived,
                            OrderQty = purchaseOrder.OrderQty,
                            Remaining = 0,
                            UpdatedDateTime = DateTime.UtcNow,
                            StatusUpdatedDateTime = DateTime.UtcNow,
                        };
                        await _context.PurchaseOrders.AddAsync(purchaseOrderEntity);
                        await _context.SaveChangesAsync();

                        currentPurchaseOrder = purchaseOrderEntity;

                    }

                    if (!String.IsNullOrEmpty(purchaseOrder.ItemSKU))
                    {
                        // check to see if item has a profile for this customer and create one if not
                        Item existingItem = await _context.Items.FirstOrDefaultAsync(item => item.SKU == purchaseOrder.ItemSKU
                                                                                        && item.CustomerId == state.CustomerId.Value
                                                                                        && !item.Deleted);
                        Item itemEntity = new Item();

                        if (existingItem == null)
                        {
                            UnitOfMeasure existingUOM = await _context.UnitOfMeasures.FirstOrDefaultAsync(uom => (uom.Code.ToLower() == purchaseOrder.ItemUoM.ToLower() || uom.Description.ToLower() == purchaseOrder.ItemUoM)
                                                                                                                && !uom.Deleted
                                                                                                                && (uom.Type == UnitOfMeasureTypeEnum.Default 
                                                                                                                    || (uom.Type == UnitOfMeasureTypeEnum.Custom && uom.CustomerId == state.CustomerId.Value)));
                            UnitOfMeasureCustomer uomCustomerEntity = new UnitOfMeasureCustomer();
                            UnitOfMeasure uomEntity = new UnitOfMeasure();

                            if (existingUOM == null)
                            {
                                uomEntity.Code = purchaseOrder.ItemUoM;
                                uomEntity.Description = purchaseOrder.ItemUoM;
                                uomEntity.Type = UnitOfMeasureTypeEnum.Custom;
                                uomEntity.CustomerId = state.CustomerId.Value;

                                await _context.UnitOfMeasures.AddAsync(uomEntity);
                                await _context.SaveChangesAsync();
                                await _context.UnitOfMeasureCustomers.AddAsync(new UnitOfMeasureCustomer
                                {
                                    CustomerId = state.CustomerId.Value,
                                    UnitOfMeasureId = uomEntity.UnitOfMeasureId
                                });
                                await _context.SaveChangesAsync();

                            } else
                            {
                                uomEntity = existingUOM;
                            }

                            itemEntity.SKU = purchaseOrder.ItemSKU;
                            itemEntity.CustomerId = state.CustomerId.Value;
                            itemEntity.Description = purchaseOrder.ItemDescription;
                            itemEntity.UnitOfMeasureId = uomEntity.UnitOfMeasureId;

                            await _context.Items.AddAsync(itemEntity);
                            await _context.SaveChangesAsync();

                            if (!String.IsNullOrEmpty(purchaseOrder.LotNumber))
                            {
                                Lot existingLot = await _context.Lots.FirstOrDefaultAsync(lot => lot.LotNo == purchaseOrder.LotNumber
                                                                                        && lot.ItemId == itemEntity.ItemId 
                                                                                        && lot.CustomerLocationId == purchaseOrder.CustomerLocationId);
                                Lot lotEntity = new Lot();

                                if (existingLot == null)
                                {
                                    lotEntity.CustomerLocationId = purchaseOrder.CustomerLocationId;
                                    lotEntity.ItemId = itemEntity.ItemId;
                                    lotEntity.LotNo = purchaseOrder.LotNumber;
                                    lotEntity.ExpirationDate = (DateTime)purchaseOrder.ExpirationDate;

                                    await _context.Lots.AddAsync(lotEntity);
                                    await _context.SaveChangesAsync();

                                } else
                                {
                                    lotEntity = existingLot;
                                }
                            }
                        }
                        else
                        {
                            itemEntity = existingItem;
                        }

                        Receive receiveEntity = new Receive()
                        {
                            CustomerLocationId = purchaseOrder.CustomerLocationId,
                            PurchaseOrderId = currentPurchaseOrder.PurchaseOrderId,
                            ItemId = itemEntity.ItemId,
                            Qty = purchaseOrder.ItemOrderQty,
                            Received = 0,
                            Remaining = purchaseOrder.ItemOrderQty,
                        };

                        receives.Add(receiveEntity);
                    }
                }

                await _context.Receives.AddRangeAsync(receives);
                await _context.SaveChangesAsync();

                return Result.Ok();

            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }



        public async Task<Result<PurchaseOrderLookupGetModel>> GetPurchaseOrderLookupAsync(AppState state, int customerFacilityId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.PurchaseOrders
                    .Include(x => x.Vendor)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerFacilityId == customerFacilityId
                        && (x.PurchaseOrderNo.Trim().ToLower().Contains(searchText)
                            || x.ShipVia.Trim().ToLower().Contains(searchText)
                            || x.Vendor.Name.Trim().ToLower().Contains(searchText)
                            || x.Vendor.City.Trim().ToLower().Contains(searchText)
                            || x.Vendor.VendorNo.Trim().ToLower().Contains(searchText)
                            || x.OrderDate.ToString().Trim().ToLower().Contains(searchText)
                            || x.OrderQty.ToString().Trim().ToLower().Contains(searchText)
                            || x.Remaining.ToString().Trim().ToLower().Contains(searchText)
                            || x.UpdatedDateTime.ToString().Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    query = query.Where(x => x.CustomerFacilityId == customerFacilityId);
                }

                var po = await query
                    .AsNoTracking()
                    .Select(x => new PurchaseOrderLookupGetModel.PurchaseOrder
                    {
                        PurchaseOrderId = x.PurchaseOrderId,
                        Status = x.Status.GetEnumDescription(),
                        PoNo = x.PurchaseOrderNo,
                        ShipVia = x.ShipVia,
                        VendorName = x.Vendor.Name,
                        VendorCity = x.Vendor.City,
                        VendorAccount = x.Vendor.VendorNo,
                        OrderDate = x.OrderDate,
                        OrderQty = x.OrderQty,
                        Remaining = x.Remaining,
                        LastUpdated = x.UpdatedDateTime
                    })
                    .ToListAsync();

                var model = new PurchaseOrderLookupGetModel();
                model.PurchaseOrderCount = po.Count();
                model.ExpectedUnits = po.Sum(x => x.OrderQty);
                model.ReceiptsToday = po.Where(x => x.OrderDate == DateTime.Now).Count();
                model.PurchaseOrders = po;

                if (model is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrder)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<PurchaseOrderWithVendorGetModel>>> GetPurchaseOrderByCustomerIdAsync(AppState state)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.CustomerLocations.AsQueryable();

                query = query
                       .Where(x => x.CustomerId == state.CustomerId);

                var customerLocationExist = await query
                    .AnyAsync();

                if (!customerLocationExist)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }


                IEnumerable<PurchaseOrderWithVendorGetModel> model = await _context.PurchaseOrders
                    .Include(x => x.CustomerLocation)
                    .Include(x => x.Vendor)
                    .Where(x => x.CustomerLocation.CustomerId == state.CustomerId)
                    .AsNoTracking()
                    .Select(x => new PurchaseOrderWithVendorGetModel
                    {
                        PurchaseOrderId = x.PurchaseOrderId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        CustomerFacilityId = x.CustomerFacilityId.Value,
                        VendorId = x.VendorId.Value,
                        VendorName = x.Vendor.Name,
                        VendorNo = x.Vendor.VendorNo,
                        VendorCity = x.Vendor.City,
                        PurchaseOrderNo = x.PurchaseOrderNo,
                        ShipVia = x.ShipVia,
                        Status = x.Status,
                        OrderDate = x.OrderDate,
                        OrderQty = x.OrderQty,
                        Remaining = x.Remaining,
                        UpdatedDateTime = x.UpdatedDateTime,
                        StatusUpdatedDateTime = x.StatusUpdatedDateTime,
                    })
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<PurchaseOrderLookupGetModel>> DeletePurchaseOrderAsync(AppState state, PurchaseOrderDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerFacilityId is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrderDeleteModel.CustomerFacilityId)} is required.");
                }

                if (model.PurchaseOrderId is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrderDeleteModel.PurchaseOrderId)} is required.");
                }

                var entity = await _context.PurchaseOrders
                    .SingleOrDefaultAsync(x => x.PurchaseOrderId == model.PurchaseOrderId
                        && x.CustomerFacilityId == model.CustomerFacilityId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrder)} not found.");
                }

                entity.Deleted = true;
                await _context.SaveChangesAsync();

                var po = await _context.PurchaseOrders
                    .Include(x => x.Vendor)
                    .Where(x => x.CustomerFacilityId == model.CustomerFacilityId)
                    .AsNoTracking()
                    .Select(x => new PurchaseOrderLookupGetModel.PurchaseOrder
                    {
                        PurchaseOrderId = x.PurchaseOrderId,
                        Status = x.Status.GetEnumDescription(),
                        PoNo = x.PurchaseOrderNo,
                        ShipVia = x.ShipVia,
                        VendorName = x.Vendor.Name,
                        VendorCity = x.Vendor.City,
                        VendorAccount = x.Vendor.VendorNo,
                        OrderDate = x.OrderDate,
                        OrderQty = x.OrderQty,
                        Remaining = x.Remaining,
                        LastUpdated = x.UpdatedDateTime
                    })
                    .ToListAsync();

                var dto = new PurchaseOrderLookupGetModel();
                dto.PurchaseOrderCount = po.Count();
                dto.ExpectedUnits = po.Sum(x => x.OrderQty);
                dto.ReceiptsToday = po.Where(x => x.OrderDate == DateTime.Now).Count();
                dto.PurchaseOrders = po;

                if (model is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrder)} not found.");
                }

                return Result.Ok(dto);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<PurchaseOrderDetailGetModel>> GetPurchaseOrderDetailAsync(AppState state, int purchaseOrderId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.PurchaseOrders
                    .Include(x => x.Receives)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.Receives)
                        .ThenInclude(x => x.Lot)
                    .AsQueryable();

                if (state.Role == RoleEnum.SuperAdmin)
                {
                    query = query
                        .Where(x => x.PurchaseOrderId == purchaseOrderId);
                }
                else
                {
                    query = query
                        .Where(x => x.PurchaseOrderId == purchaseOrderId
                            && x.CustomerLocation.CustomerId == state.CustomerId);
                }

                var model = await query
                    .AsNoTracking()
                    .Select(x => new PurchaseOrderDetailGetModel
                    {
                        PurchaseOrderDetail = new PurchaseOrderDetailGetModel.PurchaseOrder
                        {
                            PurchaseOrderId = x.PurchaseOrderId,
                            PurchaseOrderNo = x.PurchaseOrderNo,
                            Status = x.Status.GetEnumDescription(),
                            PoNo = x.PurchaseOrderNo,
                            ShipVia = x.ShipVia,
                            VendorName = x.Vendor.Name,
                            VendorAddress1 = x.Vendor.Address1,
                            VendorAddress2 = x.Vendor.Address2,
                            VendorZip = x.Vendor.ZipPostalCode,
                            VendorStateOrProvince = x.Vendor.StateProvince,
                            VendorCity = x.Vendor.City,
                            VendorAccount = x.Vendor.VendorNo,
                            VendorPhoneNumber = x.Vendor.PhoneNumber,
                            OrderDate = x.OrderDate,
                            OrderQty = x.OrderQty,
                            Remaining = x.Remaining,
                            LastUpdated = x.UpdatedDateTime
                        },
                        Items = x.Receives.Select(z => new PurchaseOrderDetailGetModel.Item
                        {
                            ReceiveId = z.ReceiveId,
                            ItemId = z.Item.ItemId,
                            ItemSKU = z.Item.SKU,
                            Description = z.Item.Description,
                            UOM = z.Item.UnitOfMeasure.Code,
                            OrderQty = z.Qty,
                            ReceivedQty = z.Received,
                            LotId = z.Lot.LotId,
                            LotNo = z.Lot.LotNo,
                            ExpirationDate = z.Lot.ExpirationDate.ToString("MM/dd/yyyy")
                        })
                    })
                    .SingleOrDefaultAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrder)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel>>> GetPurchaseOrderLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (barcodeSearch && !searchText.HasValue())
                {
                    return Result.Fail("PO No is required.");
                }

                var query = _context.PurchaseOrders
                    .Include(x => x.Vendor)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (barcodeSearch)
                    {
                        query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == customerFacilityId
                            && x.Status != PurchaseOrderStatusEnum.Closed
                            && x.PurchaseOrderNo.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == customerFacilityId
                            && x.Status != PurchaseOrderStatusEnum.Closed
                            && (x.PurchaseOrderNo.Trim().ToLower().Contains(searchText)
                                || x.Vendor.VendorNo.Trim().ToLower().Contains(searchText)
                                || x.Vendor.Name.Trim().ToLower().Contains(searchText)));
                    }
                }
                else
                {
                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerFacilityId == customerFacilityId
                        && x.Status != PurchaseOrderStatusEnum.Closed);
                }

                IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new PurchaseOrderLookupPOReceiveDeviceGetModel
                    {
                        PurchaseOrderId = x.PurchaseOrderId,
                        PurchaseOrderNo = x.PurchaseOrderNo,
                        VendorName = x.Vendor.Name,
                        VendorNo = x.Vendor.VendorNo
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrder)} not found.");
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