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
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public ItemService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<ItemGetModel>> CreateItemAsync(AppState state, ItemCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(ItemCreateModel.CustomerId)} is required.");
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

                if (!model.SKU.HasValue())
                {
                    return Result.Fail($"{nameof(ItemCreateModel.SKU)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.SKU.Trim().ToLower() == model.SKU.Trim().ToLower()
                            && x.CustomerId == model.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(ItemCreateModel.SKU)} is already exist.");
                    }
                }

                if (!model.Description.HasValue())
                {
                    return Result.Fail($"{nameof(ItemCreateModel.Description)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.Description.Trim().ToLower() == model.Description.Trim().ToLower()
                            && x.CustomerId == model.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(ItemCreateModel.Description)} is already exist.");
                    }
                }

                if (model.UOMId is null)
                {
                    return Result.Fail($"{nameof(ItemCreateModel.UOMId)} is required.");
                }

                int defaultThreshold = 0;
                int? t = await _context.Customers
                    .Where(x => x.CustomerId == model.CustomerId)
                    .Select(x => x.DefaultItemThreshold)
                    .SingleOrDefaultAsync();
                if (t != null)
                {
                    defaultThreshold = (int)t;
                }

                var entity = new Item
                {
                    CustomerId = model.CustomerId,
                    SKU = model.SKU,
                    Description = model.Description,
                    UnitOfMeasureId = model.UOMId,
                    Threshold = defaultThreshold,
                };


                _context.Add(entity);
                await _context.SaveChangesAsync();

                entity = await _context.Items
                   .Include(x => x.UnitOfMeasure)
                   .SingleOrDefaultAsync(x => x.ItemId == entity.ItemId);

                return Result.Ok(new ItemGetModel
                {
                    ItemId = entity.ItemId,
                    CustomerId = entity.CustomerId.Value,
                    SKU = entity.SKU,
                    Description = entity.Description,
                    UOM = entity.UnitOfMeasure.Code,
                    ExpirationDate = entity.ExpirationDate,
                    Threshold = entity.Threshold
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ItemGetModel>> DeleteItemAsync(AppState state, ItemDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(ItemDeleteModel.ItemId)} is required.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.Items
                            .Include(x => x.UnitOfMeasure)
                            .Include(x => x.Inventory)
                                .ThenInclude(x => x.InventoryZones)
                            .Include(x => x.Inventory)
                                .ThenInclude(x => x.InventoryBins)
                            .Include(x => x.Inventory)
                                .ThenInclude(x => x.ActivityLogs)
                            .Include(x => x.Receives)
                                .ThenInclude(x => x.PutAways)
                                    .ThenInclude(x => x.PutAwayBins)
                            .Include(x => x.ItemVendors)
                            .Include(x => x.Receipts)
                                .ThenInclude(x => x.PutAways)
                                    .ThenInclude(x => x.PutAwayBins)
                            .Include(x => x.Transfers)
                                .ThenInclude(x => x.TransferZoneBins)
                            .Include(x => x.AdjustBinQuantities)
                            .AsSplitQuery()
                            .SingleOrDefaultAsync(x => x.ItemId == model.ItemId);

                        if (entity is null)
                        {
                            return Result.Fail($"{nameof(Item)} not found.");
                        }

                        entity.Deleted = true;

                        entity.Inventory.Deleted = true;
                        foreach (var x in entity.Inventory.InventoryZones)
                        {
                            x.Deleted = true;
                        }
                        foreach (var x in entity.Inventory.InventoryBins)
                        {
                            x.Deleted = true;
                        }
                        foreach (var x in entity.Inventory.ActivityLogs)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Receives)
                        {
                            x.Deleted = true;

                            foreach (var z in x.PutAways)
                            {
                                z.Deleted = true;

                                foreach (var zz in z.PutAwayBins)
                                {
                                    zz.Deleted = true;
                                }
                            }
                        }

                        foreach (var x in entity.ItemVendors)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Receipts)
                        {
                            x.Deleted = true;

                            foreach (var z in x.PutAways)
                            {
                                z.Deleted = true;

                                foreach (var zz in z.PutAwayBins)
                                {
                                    zz.Deleted = true;
                                }
                            }
                        }

                        foreach (var x in entity.Transfers)
                        {
                            x.Deleted = true;

                            foreach (var zz in x.TransferZoneBins)
                            {
                                zz.Deleted = true;
                            }
                        }

                        foreach (var x in entity.AdjustBinQuantities)
                        {
                            x.Deleted = true;
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return Result.Ok(new ItemGetModel
                        {
                            ItemId = entity.ItemId,
                            CustomerId = entity.CustomerId.Value,
                            SKU = entity.SKU,
                            Description = entity.Description,
                            UOM = entity.UnitOfMeasure.Code,
                            ExpirationDate = entity.ExpirationDate,
                            Threshold = entity.Threshold
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

        public async Task<Result<ItemGetModel>> UpdateItemExpirationDateAsync(AppState state, ItemExpirationDateUpdateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(ItemExpirationDateUpdateModel)} is required.");
                }

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(ItemExpirationDateUpdateModel.ItemId)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.ItemId == model.ItemId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Item)} not found.");
                    }
                }

                if (model.ExpirationDate is null)
                {
                    return Result.Fail($"{nameof(ItemExpirationDateUpdateModel.ExpirationDate)} is required.");
                }

                var entity = await _context.Items
                    .Include(x => x.UnitOfMeasure)
                    .SingleOrDefaultAsync(x => x.ItemId == model.ItemId);

                entity.ExpirationDate = model.ExpirationDate;

                await _context.SaveChangesAsync();

                return Result.Ok(new ItemGetModel
                {
                    ItemId = entity.ItemId,
                    CustomerId = entity.CustomerId.Value,
                    SKU = entity.SKU,
                    Description = entity.Description,
                    UOM = entity.UnitOfMeasure.Code,
                    ExpirationDate = entity.ExpirationDate,
                    Threshold = entity.Threshold
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ItemGetModel>> UpdateItemThresholdAsync(AppState state, ItemThresholdUpdateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(ItemThresholdUpdateModel)} is required.");
                }

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(ItemThresholdUpdateModel.ItemId)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.ItemId == model.ItemId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Item)} not found.");
                    }
                }

                if (model.Threshold is null)
                {
                    return Result.Fail($"{nameof(ItemThresholdUpdateModel.Threshold)} is required.");
                }

                var entity = await _context.Items
                    .Include(x => x.UnitOfMeasure)
                    .SingleOrDefaultAsync(x => x.ItemId == model.ItemId);

                entity.Threshold = model.Threshold;

                await _context.SaveChangesAsync();

                return Result.Ok(new ItemGetModel
                {
                    ItemId = entity.ItemId,
                    CustomerId = entity.CustomerId.Value,
                    SKU = entity.SKU,
                    Description = entity.Description,
                    UOM = entity.UnitOfMeasure.Code,
                    ExpirationDate = entity.ExpirationDate,
                    Threshold = entity.Threshold
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ItemLookupGetModel>>> GetItemLookupAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                if (state.Role != RoleEnum.Viewer)
                {
                    var query = _context.Items
                        .Include(x => x.UnitOfMeasure)
                        .Include(x => x.ItemVendors)
                        .Include(x => x.Inventory)
                            .ThenInclude(x => x.InventoryBins)
                        .AsQueryable();

                    if (searchText.HasValue())
                    {
                        searchText = searchText.Trim().ToLower();

                        query = query.Where(x => x.CustomerId == customerId
                            && (x.SKU.Trim().ToLower().Contains(searchText)
                                || x.UnitOfMeasure.Code.Contains(searchText)
                                || x.Description.Trim().ToLower().Contains(searchText)));
                    }
                    else
                    {
                        query = query.Where(x => x.CustomerId == customerId);
                    }

                    IEnumerable<ItemLookupGetModel> model = await query
                        .AsNoTracking()
                        .Select(x => new ItemLookupGetModel
                        {
                            ItemId = x.ItemId,
                            ItemSKU = x.SKU,
                            UOM = x.UnitOfMeasure.Code,
                            BinLocations = x.Inventory == null ? 0 : x.Inventory.InventoryBins.Count(),
                            QtyOnHand = x.Inventory == null ? 0 : x.Inventory.QtyOnHand,
                            Description = x.Description,
                            Vendors = x.Inventory == null ? 0 : x.ItemVendors.Count(),
                            ExpirationDate = x.ExpirationDate,
                            Threshold = x.Threshold
                        })
                        .ToListAsync();

                    if (model is null)
                    {
                        return Result.Fail($"Item not found.");
                    }

                    return Result.Ok(model);
                }
                else
                {
                    var userVendors = new List<int>();
                    if (state.Role == RoleEnum.Viewer)
                    {
                        userVendors = await _context.UserRoleVendors
                            .AsNoTracking()
                            .Where(x => x.UserId == state.UserId
                                && x.RoleId == RoleEnum.Viewer.ToInt())
                            .Select(x => x.VendorId.Value)
                            .ToListAsync();
                    }

                    var query = _context.ItemVendors
                        .Include(x => x.Item)
                            .ThenInclude(x => x.UnitOfMeasure)
                        .Include(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryBins)
                        .AsQueryable();

                    if (searchText.HasValue())
                    {
                        searchText = searchText.Trim().ToLower();

                        query = query.Where(x => x.CustomerId == customerId
                            && userVendors.Contains(x.VendorId.Value)
                            && (x.Item.SKU.Trim().ToLower().Contains(searchText)
                                || x.Item.UnitOfMeasure.Code.Contains(searchText)
                                || x.Item.Description.Trim().ToLower().Contains(searchText)));
                    }
                    else
                    {
                        query = query.Where(x => x.CustomerId == customerId
                            && userVendors.Contains(x.VendorId.Value));
                    }

                    IEnumerable<ItemLookupGetModel> model = await query
                        .AsNoTracking()
                        .Select(x => new ItemLookupGetModel
                        {
                            ItemId = x.Item.ItemId,
                            ItemSKU = x.Item.SKU,
                            UOM = x.Item.UnitOfMeasure.Code,
                            BinLocations = x.Item.Inventory == null ? 0 : x.Item.Inventory.InventoryBins.Count(),
                            QtyOnHand = x.Item.Inventory == null ? 0 : x.Item.Inventory.QtyOnHand,
                            Description = x.Item.Description,
                            Vendors = x.Item.Inventory == null ? 0 : x.Item.ItemVendors.Count(),
                            ExpirationDate = x.Item.ExpirationDate,
                            Threshold = x.Item.Threshold
                        })
                        .ToListAsync();

                    if (model is null)
                    {
                        return Result.Fail($"Item not found.");
                    }

                    return Result.Ok(model);
                }
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ItemLookupDetailBasicGetModel>>> GetItemLookupDetailBasicAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Items
                    .Include(x => x.UnitOfMeasure)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == customerId
                        && (x.SKU.Trim().ToLower().Contains(searchText)
                            || x.Description.Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }

                IEnumerable<ItemLookupDetailBasicGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ItemLookupDetailBasicGetModel
                    {
                        ItemId = x.ItemId,
                        SKU = x.SKU,
                        Description = x.Description,
                        UOM = x.UnitOfMeasure.Code
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ItemDetailGetModel>> GetItemDetailAsync(AppState state, int customerId, int itemId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                if (state.Role == RoleEnum.Viewer)
                {
                    var userVendors = await _context.UserRoleVendors
                        .AsNoTracking()
                        .Where(x => x.UserId == state.UserId
                            && x.RoleId == RoleEnum.Viewer.ToInt())
                        .Select(x => x.VendorId.Value)
                        .ToListAsync();

                    foreach (var x in userVendors)
                    {
                        var exist = await _context.ItemVendors
                            .AnyAsync(z => z.VendorId == x
                                && z.ItemId == itemId
                                && z.CustomerId == customerId);

                        if (!exist)
                        {
                            return Result.Fail($"{nameof(Item)} not found.");
                        }
                    }
                }

                var bins = _context.InventoryBins
                    .AsNoTracking()
                    .Include(x => x.Inventory)
                    .Include(x => x.CustomerLocation)
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                            .ThenInclude(x => x.CustomerFacility)
                    .Include(x => x.Lot)
                    .Where(x => x.Inventory.ItemId == itemId
                        && x.CustomerLocation.CustomerId == customerId);

                var customerLocations = new List<ItemDetailGetModel.CustomerLocation>();
                foreach (var x in bins)
                {
                    customerLocations.Add(new ItemDetailGetModel.CustomerLocation
                    {
                        CustomerLocationId = x.CustomerLocationId.Value,
                        CustomerFacilityId = x.Bin.Zone.CustomerFacility.CustomerFacilityId,
                        Facility = x.Bin.Zone.CustomerFacility.Name,
                        ZoneId = x.Bin.Zone.ZoneId,
                        Zone = x.Bin.Zone.Name,
                        BinId = x.Bin.BinId,
                        Bin = x.Bin.Name,
                        QtyOnHand = x.Qty,
                        LotNo = x.Lot?.LotNo,
                        ExpirationDate = x.Lot?.ExpirationDate.ToString("MM/dd/yyyy")
                    });
                }

                var vendors = await _context.ItemVendors
                    .AsNoTracking()
                    .Include(x => x.Vendor)
                    .Where(x => x.ItemId == itemId && x.CustomerId == customerId)
                    .Select(x => new ItemDetailGetModel.Vendor
                    {
                        VendorId = x.Vendor.VendorId,
                        VendorNo = x.Vendor.VendorNo,
                        Name = x.Vendor.Name,
                        Contact = x.Vendor.PointOfContact,
                        Address = x.Vendor.Address1,
                        Phone = x.Vendor.PhoneNumber
                    })
                    .ToListAsync();

                var activityLogs = await _context.ActivityLogs
                    .AsNoTracking()
                    .Include(x => x.Inventory)
                    .Include(x => x.User)
                        .ThenInclude(x => x.Role)
                    .Include(x => x.Zone)
                    .Include(x => x.Bin)
                    .Where(x => x.Inventory.ItemId == itemId && x.CustomerId == customerId)
                    .OrderByDescending(x => x.ActivityDateTime)
                    .ToListAsync();

                var activities = new List<ItemDetailGetModel.Activity>();
                foreach (var x in activityLogs)
                {
                    var dto = new ItemDetailGetModel.Activity();
                    dto.ActivityLogId = x.ActivityLogId;

                    if (x.Type == ActivityLogTypeEnum.AddBin)
                    {
                        dto.Type = "Inventory (Add Bin & Qty)";
                        dto.Qty = $"+{x.Qty}";
                    }
                    else if (x.Type == ActivityLogTypeEnum.Adjustment)
                    {
                        dto.Type = "Adjustment";
                        dto.Qty = $"Adjust qty from {x.OldQty} to {x.NewQty}";
                    }
                    else if (x.Type == ActivityLogTypeEnum.PurchaseOrder)
                    {
                        dto.Type = "Purchase Order";
                        dto.Qty = $"+{x.Qty}";
                    }
                    else if (x.Type == ActivityLogTypeEnum.Receipt)
                    {
                        dto.Type = "Receipt";
                        dto.Qty = $"+{x.Qty}";
                    }
                    else if (x.Type == ActivityLogTypeEnum.SaleOrder)
                    {
                        dto.Type = "Sale Order";
                        dto.Qty = $"-{x.Qty}";
                    }
                    else if (x.Type == ActivityLogTypeEnum.Transfer)
                    {
                        dto.Type = "Transfer";
                        dto.Qty = $"{x.Qty}";
                    }
                    else if (x.Type == ActivityLogTypeEnum.TransferManual)
                    {
                        dto.Type = "Transfer (Manual)";
                        dto.Qty = $"{x.Qty}";
                    }
                    else if (x.Type == ActivityLogTypeEnum.Recall)
                    {
                        dto.Type = "Recall";
                        dto.Qty = $"-{x.Qty}";
                    }

                    dto.User = $"{x.User.Name} ({x.User.Role.Name})";
                    dto.Date = x.ActivityDateTime.ToString("MM/dd/yyyy, h:mm tt");
                    dto.Zone = x.Zone.Name;
                    dto.BinLocation = x.Bin.Name;

                    activities.Add(dto);
                }

                var model = new ItemDetailGetModel();
                model.CustomerLocations = customerLocations;
                model.Vendors = vendors;
                model.Activities = activities;

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ItemVendorGetModel>>> GetItemByVendorIdAsync(AppState state, int vendorId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var bins = _context.InventoryBins
                    .AsNoTracking()
                    .Include(x => x.Inventory)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.ItemVendors)
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                            .ThenInclude(x => x.CustomerFacility)
                    .Where(x => x.Inventory.Item.ItemVendors.Any(x => x.VendorId == vendorId));

                var customerLocations = new List<ItemDetailGetModel.CustomerLocation>();
                foreach (var x in bins)
                {
                    customerLocations.Add(new ItemDetailGetModel.CustomerLocation
                    {
                        CustomerLocationId = x.CustomerLocationId.Value,
                        CustomerFacilityId = x.Bin.Zone.CustomerFacility.CustomerFacilityId,
                        Facility = x.Bin.Zone.CustomerFacility.Name,
                        ZoneId = x.Bin.Zone.ZoneId,
                        Zone = x.Bin.Zone.Name,
                        BinId = x.Bin.BinId,
                        Bin = x.Bin.Name,
                        QtyOnHand = x.Qty
                    });
                }

                var query = _context.InventoryBins
                    .Include(x => x.Inventory)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.ItemVendors)
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                            .ThenInclude(x => x.CustomerFacility)
                    .AsQueryable();

                IEnumerable<ItemVendorGetModel> model;
                if (state.Role == RoleEnum.SuperAdmin)
                {
                    model = await query
                        .AsNoTracking()
                        .Where(x => x.Inventory.Item.ItemVendors.Any(x => x.VendorId == vendorId))
                        .Select(x => new ItemVendorGetModel
                        {
                            ItemId = x.Inventory.Item.ItemId,
                            ItemSKU = x.Inventory.Item.SKU,
                            Facility = x.Bin.Zone.CustomerFacility.Name,
                            Zone = x.Bin.Zone.Name,
                            BinId = x.Bin.BinId,
                            Bin = x.Bin.Name,
                            QtyOnHand = x.Qty,
                            Description = x.Inventory.Item.Description,
                            Vendors = x.Inventory.Item.ItemVendors.Count()
                        })
                        .ToListAsync();
                }
                else
                {
                    model = await query
                        .AsNoTracking()
                        .Where(x => x.Inventory.Item.ItemVendors.Any(x => x.VendorId == vendorId)
                            && x.Inventory.Item.CustomerId == state.CustomerId)
                        .Select(x => new ItemVendorGetModel
                        {
                            ItemId = x.Inventory.Item.ItemId,
                            ItemSKU = x.Inventory.Item.SKU,
                            Facility = x.Bin.Zone.CustomerFacility.Name,
                            Zone = x.Bin.Zone.Name,
                            BinId = x.Bin.BinId,
                            Bin = x.Bin.Name,
                            QtyOnHand = x.Qty,
                            Description = x.Inventory.Item.Description,
                            Vendors = x.Inventory.Item.ItemVendors.Count()
                        })
                        .ToListAsync();
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(Vendor)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ItemSkuLookupGetModel>>> GetItemLookupBySkuAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Items
                    .AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == customerId
                        && x.SKU.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }

                IEnumerable<ItemSkuLookupGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ItemSkuLookupGetModel
                    {
                        ItemId = x.ItemId,
                        SKU = x.SKU
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ItemSkuLookupGetModel>> GetItemSkuLookupAsync(AppState state, int customerId, string sku)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (!sku.HasValue())
                {
                    return Result.Fail("Item SKU is required.");
                }

                var query = _context.Items
                    .AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                sku = sku.Trim().ToLower();

                var model = await query
                    .AsNoTracking()
                    .Where(x => x.CustomerId == customerId
                        && x.SKU.Trim().ToLower().Contains(sku))
                    .Select(x => new ItemSkuLookupGetModel
                    {
                        ItemId = x.ItemId,
                        SKU = x.SKU
                    })
                    .SingleOrDefaultAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ItemManualReceiptLookupGetModel>>> GetItemManualReceiptLookupAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Items
                    .AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == customerId
                        && (x.SKU.Trim().ToLower().Contains(searchText)
                            || x.Description.Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }

                IEnumerable<ItemManualReceiptLookupGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ItemManualReceiptLookupGetModel
                    {
                        ItemId = x.ItemId,
                        ItemSKU = x.SKU,
                        Description = x.Description
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ItemPurchaseOrderLookupGetModel>>> GetItemPurchaseOrderLookupAsync(AppState state, int purchaseOrderId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.PurchaseOrders.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query
                        .Include(x => x.CustomerLocation)
                        .Where(x => x.PurchaseOrderId == purchaseOrderId
                            && x.CustomerLocation.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query
                        .Where(x => x.PurchaseOrderId == purchaseOrderId);
                }

                var po = await query
                    .AsNoTracking()
                    .Select(x => x.VendorId)
                    .SingleOrDefaultAsync();

                if (po is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrder)} not found.");
                }

                var queryItem = _context.ItemVendors
                        .Include(x => x.Item)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    queryItem = queryItem.Where(x => x.VendorId == po
                        && (x.Item.SKU.Trim().ToLower().Contains(searchText)
                            || x.Item.Description.Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    queryItem = queryItem.Where(x => x.VendorId == po);
                }

                IEnumerable<ItemPurchaseOrderLookupGetModel> model = await queryItem
                    .AsNoTracking()
                    .Select(x => new ItemPurchaseOrderLookupGetModel
                    {
                        ItemId = x.Item.ItemId,
                        SKU = x.Item.SKU,
                        Description = x.Item.Description
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ItemLookupDeviceGetModel>>> GetItemLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Inventories
                    .Include(x => x.InventoryZones)
                        .ThenInclude(x => x.Zone)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Receives)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == state.CustomerId
                        && x.InventoryZones.Any(z => z.Zone.CustomerFacilityId == customerFacilityId)
                        && (x.Item.SKU.Trim().ToLower().Contains(searchText)
                            || x.Item.Description.Trim().ToLower().Contains(searchText)
                            || x.Item.Inventory.QtyOnHand.ToString().Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == state.CustomerId
                        && x.InventoryZones.Any(z => z.Zone.CustomerFacilityId == customerFacilityId));
                }

                IEnumerable<ItemLookupDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ItemLookupDeviceGetModel
                    {
                        ItemId = x.Item.ItemId,
                        SKU = x.Item.SKU,
                        Description = x.Item.Description,
                        UOM = x.Item.UnitOfMeasure.Code,
                        Barcode = x.Item.Barcode,
                        QtyOnHand = x.QtyOnHand,
                        OnOrder = x.Item.Receives.Sum(x => x.Remaining) + x.Item.Receipts.Sum(x => x.Remaining)
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ItemLookupDeviceGetModel>> GetItemLookupSkuDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string sku)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (!sku.HasValue())
                {
                    return Result.Fail("Item SKU is required.");
                }

                var model = await _context.Inventories
                    .Include(x => x.InventoryZones)
                        .ThenInclude(x => x.Zone)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Receives)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .AsNoTracking()
                    .Where(x => x.CustomerId == state.CustomerId
                        && x.InventoryZones.Any(z => z.Zone.CustomerFacilityId == customerFacilityId)
                        && x.Item.SKU.Trim() == sku.Trim())
                    .Select(x => new ItemLookupDeviceGetModel
                    {
                        ItemId = x.Item.ItemId,
                        SKU = x.Item.SKU,
                        Description = x.Item.Description,
                        UOM = x.Item.UnitOfMeasure.Code,
                        Barcode = x.Item.Barcode,
                        QtyOnHand = x.InventoryZones
                            .Where(z => z.Zone.CustomerFacilityId == customerFacilityId).Sum(x => x.Qty),
                        OnOrder = x.Item.Receives.Sum(x => x.Qty)
                    })
                    .SingleOrDefaultAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
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
