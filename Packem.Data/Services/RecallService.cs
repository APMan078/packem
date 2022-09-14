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
    public class RecallService : IRecallService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public RecallService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<RecallGetModel>> CreateRecallAsync(AppState state, RecallCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(RecallCreateModel)} is required.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(RecallCreateModel.CustomerId)} is required.");
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

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(OrderLineCreateModel.ItemId)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.ItemId == model.ItemId
                            && x.CustomerId == model.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Item)} not found.");
                    }
                }

                var recall = await _context.Recalls
                    .AnyAsync(x => x.ItemId == model.ItemId
                        &&x.CustomerFacilityId == model.CustomerFacilityId
                        && x.CustomerLocationId == model.CustomerLocationId
                        && x.Status == RecallStatusEnum.Pending);

                if (recall)
                {
                    return Result.Fail($"This item have already a pending recall.");
                }

                var entity = new Recall
                {
                    CustomerLocationId = model.CustomerLocationId,
                    CustomerFacilityId = model.CustomerFacilityId,
                    ItemId = model.ItemId,
                    RecallDate = DateTime.Now,
                    Status = RecallStatusEnum.Pending
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new RecallGetModel
                {
                    RecallId = entity.RecallId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    CustomerFacilityId = entity.CustomerFacilityId.Value,
                    ItemId = entity.ItemId.Value,
                    RecallDate = entity.RecallDate,
                    Status = entity.Status
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<RecallQueueGetModel>>> GetRecallQueueAsync(AppState state, int customerLocationId, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                IEnumerable<RecallQueueGetModel> model = await _context.Recalls
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                    .Include(x => x.RecallBins)
                    .AsNoTracking()
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId
                        && x.Status != RecallStatusEnum.Complete)
                    .Select(x => new RecallQueueGetModel
                    {
                        RecallId = x.RecallId,
                        ItemId = x.Item.ItemId,
                        ItemSKU = x.Item.SKU,
                        ItemDescription = x.Item.Description,
                        ItemUOM = x.Item.UnitOfMeasure.Code,
                        RecallDate = x.RecallDate,
                        Qty = x.Item.Inventory.InventoryBins.Where(z => z.Bin.Zone.CustomerFacilityId == customerFacilityId).Sum(z => z.Qty) + x.RecallBins.Sum(x => x.Qty),
                        Received = x.RecallBins.Sum(x => x.Qty)
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Recall)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<RecallGetModel>> UpdateRecallStatusDeviceAsync(CustomerDeviceTokenAuthModel state, RecallStatusUpdateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(RecallStatusUpdateModel)} is required.");
                }

                if (model.RecallId is null)
                {
                    return Result.Fail($"{nameof(RecallStatusUpdateModel.RecallId)} is required.");
                }
                else
                {
                    var exist = await _context.Recalls
                        .AnyAsync(x => x.RecallId == model.RecallId
                            && x.CustomerLocationId == state.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Recall)} not found.");
                    }
                }

                if (model.Status is null)
                {
                    return Result.Fail($"{nameof(RecallStatusUpdateModel.Status)} is required.");
                }

                var entity = await _context.Recalls
                    .SingleOrDefaultAsync(x => x.RecallId == model.RecallId
                            && x.CustomerLocationId == state.CustomerLocationId);

                entity.Status = model.Status.Value;

                await _context.SaveChangesAsync();

                return Result.Ok(new RecallGetModel
                {
                    RecallId = entity.RecallId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    CustomerFacilityId = entity.CustomerFacilityId.Value,
                    ItemId = entity.ItemId.Value,
                    RecallDate = entity.RecallDate,
                    Status = entity.Status
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<RecallQueueLookupGetModel>>> GetRecallQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (barcodeSearch && !searchText.HasValue())
                {
                    return Result.Fail("Item SKU is required.");
                }

                var query = _context.Recalls
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.InventoryBins)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (barcodeSearch)
                    {
                        query = query.Where(x => x.Status != RecallStatusEnum.Complete
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == customerFacilityId
                            && x.Item.SKU.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => x.Status != RecallStatusEnum.Complete
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == customerFacilityId
                            && x.Item.SKU.Trim().ToLower().Contains(searchText));
                    }
                }
                else
                {
                    query = query.Where(x => x.Status != RecallStatusEnum.Complete
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerFacilityId == customerFacilityId);
                }

                IEnumerable<RecallQueueLookupGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new RecallQueueLookupGetModel
                    {
                        RecallId = x.RecallId,
                        RecallDate = x.RecallDate,
                        Status = x.Status,
                        ItemSKU = x.Item.SKU,
                        ItemDescription = x.Item.Description,
                        ItemUOM = x.Item.UnitOfMeasure.Code,
                        Bins = x.Item.Inventory.InventoryBins.Where(z => z.Bin.Zone.CustomerFacilityId == x.CustomerFacilityId).Count()
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

        public async Task<Result<IEnumerable<RecallDetailGetModel>>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var recall = await _context.Recalls
                    .Include(x => x.RecallBins)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.RecallId == recallId);

                if (recall is null)
                {
                    return Result.Fail($"{nameof(Recall)} not found.");
                }

                IEnumerable<RecallDetailGetModel> model = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                    .Where(x => x.Inventory.ItemId == recall.ItemId
                        && x.Bin.Zone.CustomerFacilityId == recall.CustomerFacilityId)
                    .Select(x => new RecallDetailGetModel
                    {
                        ZoneId = x.Bin.Zone.ZoneId,
                        ZoneName = x.Bin.Zone.Name,
                        BinId = x.Bin.BinId,
                        BinName = x.Bin.Name,
                        Qty = x.Qty
                    })
                    .ToListAsync();

                foreach (var x in model)
                {
                    x.Received = recall.RecallBins.Where(z => z.BinId == x.BinId).Sum(z => z.Qty);
                    x.Qty = x.Qty + x.Received;
                    x.Remaining = x.Qty - x.Received;
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<RecallDetailGetModel>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId, int binId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var recall = await _context.Recalls
                    .Include(x => x.RecallBins)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.RecallId == recallId);

                if (recall is null)
                {
                    return Result.Fail($"{nameof(Recall)} not found.");
                }

                var model = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                    .Where(x => x.Inventory.ItemId == recall.ItemId
                        && x.BinId == binId
                        && x.Bin.Zone.CustomerFacilityId == recall.CustomerFacilityId)
                    .Select(x => new RecallDetailGetModel
                    {
                        ZoneId = x.Bin.Zone.ZoneId,
                        ZoneName = x.Bin.Zone.Name,
                        BinId = x.Bin.BinId,
                        BinName = x.Bin.Name,
                        Qty = x.Qty
                    })
                    .SingleOrDefaultAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Bin)} not found.");
                }

                model.Received = recall.RecallBins.Where(z => z.BinId == model.BinId).Sum(z => z.Qty);
                model.Qty = model.Qty + model.Received;
                model.Remaining = model.Qty - model.Received;

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<RecallBinGetModel>> CreateRecallBinDeviceAsync(CustomerDeviceTokenAuthModel state, RecallBinCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(RecallBinCreateModel)} is required.");
                }

                var existCustomer = await _context.Customers
                    .AnyAsync(x => x.CustomerId == state.CustomerId);

                if (!existCustomer)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                var existCustomerLocation = await _context.CustomerLocations
                    .AnyAsync(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerId == state.CustomerId);

                if (!existCustomerLocation)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                if (model.UserId is null)
                {
                    return Result.Fail($"{nameof(TransferManualCreateModel.UserId)} is required.");
                }
                else
                {
                    var exist = await _context.Users
                        .AnyAsync(x => x.UserId == model.UserId
                            && x.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(User)} not found.");
                    }
                }

                if (model.RecallId is null)
                {
                    return Result.Fail($"{nameof(RecallBinCreateModel.RecallId)} is required.");
                }
                else
                {
                    var exist = await _context.Recalls
                        .AnyAsync(x => x.RecallId == model.RecallId
                            && x.Status != RecallStatusEnum.Complete
                            && x.CustomerLocationId == state.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Recall)} not found.");
                    }
                }

                if (model.BinId is null)
                {
                    return Result.Fail($"{nameof(RecallBinCreateModel.BinId)} is required.");
                }
                else
                {
                    var exist = await _context.Bins
                        .AnyAsync(x => x.BinId == model.BinId
                            && x.CustomerLocationId == state.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"Location not found.");
                    }
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(RecallBinCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(RecallBinCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(RecallBinCreateModel.Qty)} cannot be zero.");
                    }
                }

                var recall = await _context.Recalls
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                    .Include(x => x.RecallBins)
                    .AsNoTracking()
                    .Where(x => x.RecallId == model.RecallId
                        && x.Status != RecallStatusEnum.Complete)
                    .Select(x => new RecallQueueGetModel
                    {
                        RecallId = x.RecallId,
                        ItemId = x.Item.ItemId,
                        ItemSKU = x.Item.SKU,
                        ItemDescription = x.Item.Description,
                        ItemUOM = x.Item.UnitOfMeasure.Code,
                        RecallDate = x.RecallDate,
                        Qty = x.Item.Inventory.InventoryBins.Where(z => z.Bin.Zone.CustomerFacilityId == x.CustomerLocationId).Sum(z => z.Qty) + x.RecallBins.Sum(x => x.Qty),
                        Received = x.RecallBins.Sum(x => x.Qty)
                    })
                    .SingleOrDefaultAsync();

                var remaining = recall.Qty - recall.Received;

                if (model.Qty > remaining)
                {
                    return Result.Fail($"Item's quantity to recall have remaining of: {remaining}. Cannot recall more than that.");
                }

                var bin = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BinId == model.BinId
                        && x.Inventory.ItemId == recall.ItemId
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (bin == null)
                {
                    return Result.Fail($"Item doesn't exist in this location");
                }
                else
                {
                    if (model.Qty > bin.Qty)
                    {
                        return Result.Fail($"Cannot recall more than the quantity of the item in the selected location which only have: {bin.Qty}.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = new RecallBin
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            RecallId = model.RecallId,
                            BinId = model.BinId,
                            Qty = model.Qty.Value,
                            PickDateTime = DateTime.Now
                        };
                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        // decrease qty of current bin, zone and inventory
                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == recall.ItemId);

                        var oldQty = inventory.QtyOnHand;

                        var currentInventoryBin = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .SingleOrDefaultAsync(x => x.BinId == model.BinId
                                && x.Inventory.ItemId == inventory.ItemId
                                && x.CustomerLocationId == state.CustomerLocationId);
                        currentInventoryBin.Qty = currentInventoryBin.Qty - model.Qty.Value;
                        await _context.SaveChangesAsync();

                        // delete current if zero
                        if (currentInventoryBin.Qty.IsZero())
                        {
                            currentInventoryBin.Deleted = true;
                            await _context.SaveChangesAsync();
                        }

                        // sum all bins of selected zoneId
                        var currentInventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == currentInventoryBin.InventoryId
                                && x.ZoneId == currentInventoryBin.Bin.ZoneId);

                        var sumCurrentInventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == inventory.ItemId
                                && x.Bin.ZoneId == currentInventoryZone.ZoneId)
                            .SumAsync(x => x.Qty);

                        currentInventoryZone.Qty = sumCurrentInventoryBinQty;
                        await _context.SaveChangesAsync();

                        // delete current if zero
                        if (currentInventoryZone.Qty.IsZero())
                        {
                            currentInventoryZone.Deleted = true;
                            await _context.SaveChangesAsync();
                        }

                        // sum all zones of selected inventory
                        var inventoryZoneQty = await _context.InventoryZones
                            .Include(x => x.Inventory)
                            .Where(x => x.Inventory.ItemId == inventory.ItemId)
                            .SumAsync(x => x.Qty);

                        // sum selected inventory in pallet
                        var inventoryPalletQty = await _context.PalletInventories
                            .Include(x => x.Inventory)
                            .Where(x => x.Inventory.ItemId == inventory.ItemId)
                            .SumAsync(x => x.Qty);

                        var inventoryQty = inventoryZoneQty + inventoryPalletQty;

                        inventory.QtyOnHand = inventoryQty;

                        await _context.SaveChangesAsync();

                        var currentBin = await _context.Bins
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.BinId == model.BinId);

                        // create log
                        var activityLog = new ActivityLog
                        {
                            CustomerId = state.CustomerId,
                            Type = ActivityLogTypeEnum.Recall,
                            InventoryId = inventory.InventoryId,
                            UserId = model.UserId,
                            ActivityDateTime = DateTime.Now,
                            Qty = model.Qty.Value,
                            OldQty = oldQty,
                            NewQty = inventoryQty,
                            MathematicalSymbol = MathematicalSymbolEnum.Minus,
                            ZoneId = currentBin.ZoneId,
                            BinId = currentBin.BinId
                        };
                        _context.Add(activityLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new RecallBinGetModel
                        {
                            RecallBinId = entity.RecallBinId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            RecallId = entity.RecallId.Value,
                            BinId = entity.BinId.Value,
                            Qty = model.Qty.Value,
                            PickDateTime = entity.PickDateTime
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
    }
}