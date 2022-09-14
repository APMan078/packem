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
    public class PutAwayService : IPutAwayService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public PutAwayService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<PutAwayQueueGetModel>> GetPutAwayQueueAsync(AppState state, int customerLocationId, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                IEnumerable<PutAwayQueueGetModel.PutAway> pa = await _context.PutAways
                    .Include(x => x.Receive)
                        .ThenInclude(x => x.Item)
                    .Include(x => x.Receive)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.CustomerLocation)
                        .ThenInclude(x => x.CustomerFacilities)
                    .AsNoTracking()
                    .Where(x => x.Remaining > 0
                        && x.CustomerLocationId == customerLocationId
                        && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                        && (x.PutAwayType == PutAwayTypeEnum.Receive || x.PutAwayType == PutAwayTypeEnum.Receipt))
                    .Select(x => new PutAwayQueueGetModel.PutAway
                    {
                        PutAwayId = x.PutAwayId,
                        ItemId = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.ItemId : x.Receipt.Item.ItemId,
                        SKU = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.SKU : x.Receipt.Item.SKU,
                        UOM = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.UnitOfMeasure.Code : x.Receipt.Item.UnitOfMeasure.Code,
                        Description = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.Description : x.Receipt.Item.Description,
                        Qty = x.Qty,
                        Remaining = x.Remaining,
                        //PutAwayType = x.PutAwayType,
                        ReceivedTime = x.PutAwayDate
                    })
                    .ToListAsync();

                var model = new PutAwayQueueGetModel();
                model.Items = pa.Count();
                model.PutAways = pa;

                if (model is null)
                {
                    return Result.Fail($"{nameof(PutAway)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<PutAwayBinGetModel>> CreatePutAwayBinAsync(AppState state, PutAwayBinCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinCreateModel)} is required.");
                }

                int? customerId;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId;
                }
                else
                {
                    customerId = model.CustomerId;
                }

                if (customerId is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinCreateModel.CustomerId)} is required.");
                }
                else
                {
                    var exist = await _context.Customers
                        .AnyAsync(x => x.CustomerId == customerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinCreateModel.CustomerLocationId)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerLocations
                        .AnyAsync(x => x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == customerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                if (model.UserId is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinCreateModel.UserId)} is required.");
                }
                else
                {
                    var exist = await _context.Users
                        .AnyAsync(x => x.UserId == model.UserId
                            && x.CustomerId == customerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(User)} not found.");
                    }
                }

                if (model.PutAwayId is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinCreateModel.PutAwayId)} is required.");
                }
                else
                {
                    var exist = await _context.PutAways
                        .AnyAsync(x => x.PutAwayId == model.PutAwayId
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(PutAway)} not found.");
                    }
                }

                if (model.BinId is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinCreateModel.BinId)} is required.");
                }
                else
                {
                    var exist = await _context.Bins
                        .AnyAsync(x => x.BinId == model.BinId
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"Location not found.");
                    }
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(PutAwayBinCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(PutAwayBinCreateModel.Qty)} cannot be zero.");
                    }
                }

                var putAway = await _context.PutAways
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.PutAwayId,
                        x.CustomerLocationId,
                        x.Remaining,
                        x.PutAwayType
                    })
                    .SingleOrDefaultAsync(x => x.PutAwayId == model.PutAwayId
                        && x.CustomerLocationId == model.CustomerLocationId);

                if (model.Qty > putAway.Remaining)
                {
                    return Result.Fail($"Item's quantity to put away have remaining of: {putAway.Remaining}. Cannot put away more than that.");
                }

                var palletCheck = await _context.Pallets
                    .AsNoTracking()
                    .AnyAsync(x => x.BinId == model.BinId);

                if (palletCheck)
                {
                    return Result.Fail($"Cannot put item in this location. 'Each' and 'Pallet' cannot be in same location.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Bin binDb = await _context.Bins
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.BinId == model.BinId
                                && x.CustomerLocationId == model.CustomerLocationId);

                        var entity = new PutAwayBin
                        {
                            CustomerLocationId = model.CustomerLocationId,
                            PutAwayId = model.PutAwayId,
                            BinId = binDb.BinId,
                            Qty = model.Qty.Value,
                            ReceivedDateTime = DateTime.Now
                        };
                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var selectedPutAwayBinQty = await _context.PutAwayBins
                            .Where(x => x.PutAwayId == model.PutAwayId)
                            .SumAsync(x => x.Qty);

                        // update current putaway remaining
                        var putAwayDb = await _context.PutAways
                            .SingleOrDefaultAsync(x => x.PutAwayId == model.PutAwayId);
                        putAwayDb.Remaining = putAwayDb.Qty - selectedPutAwayBinQty;
                        await _context.SaveChangesAsync();

                        // update receive remaining and received
                        var putAwayBinQty = await _context.PutAwayBins
                            .Include(x => x.PutAway)
                            .Where(x => putAway.PutAwayType == PutAwayTypeEnum.Receive ? x.PutAway.ReceiveId == putAwayDb.ReceiveId
                                : x.PutAway.ReceiptId == putAwayDb.ReceiptId)
                            .SumAsync(x => x.Qty);

                        int itemId;
                        if (putAway.PutAwayType == PutAwayTypeEnum.Receive)
                        {
                            var receive = await _context.Receives
                                .SingleOrDefaultAsync(x => x.ReceiveId == putAwayDb.ReceiveId);
                            receive.Remaining = receive.Qty - putAwayBinQty;
                            receive.Received = putAwayBinQty;
                            await _context.SaveChangesAsync();

                            // update purchase order remaining
                            var receiveRemaining = await _context.Receives
                                .Where(x => x.PurchaseOrderId == receive.PurchaseOrderId)
                                .SumAsync(x => x.Remaining);

                            var purchaseOrder = await _context.PurchaseOrders
                                .SingleOrDefaultAsync(x => x.PurchaseOrderId == receive.PurchaseOrderId);

                            purchaseOrder.Remaining = receiveRemaining;

                            if (purchaseOrder.Remaining == 0)
                            {
                                purchaseOrder.Status = PurchaseOrderStatusEnum.Closed;
                                purchaseOrder.UpdatedDateTime = DateTime.Now;
                            }

                            await _context.SaveChangesAsync();

                            itemId = receive.ItemId.Value;
                        }
                        else
                        {
                            // update current receipt remaining
                            var receiptDb = await _context.Receipts
                                .SingleOrDefaultAsync(x => x.ReceiptId == putAwayDb.ReceiptId);
                            receiptDb.Remaining = receiptDb.Qty - putAwayBinQty;
                            await _context.SaveChangesAsync();

                            itemId = receiptDb.ItemId.Value;
                        }

                        // update inventory qty, zone qty and bin qty
                        var inventoryExist = false;
                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == itemId);

                        var oldQty = 0;
                        var inventoryQty = 0;

                        if (inventory is null) // create
                        {
                            inventory = new Inventory
                            {
                                CustomerId = customerId,
                                ItemId = itemId,
                                QtyOnHand = model.Qty.Value
                            };
                            _context.Add(inventory);
                            await _context.SaveChangesAsync();

                            oldQty = model.Qty.Value;
                            inventoryQty = model.Qty.Value;
                        }
                        else
                        {
                            inventoryExist = true;
                            oldQty = inventory.QtyOnHand;
                        }

                        var inventoryBin = await _context.InventoryBins
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.BinId == binDb.BinId
                                && x.CustomerLocationId == model.CustomerLocationId);

                        if (inventoryBin is null)
                        {
                            inventoryBin = new InventoryBin
                            {
                                CustomerLocationId = model.CustomerLocationId,
                                InventoryId = inventory.InventoryId,
                                BinId = binDb.BinId,
                                Qty = model.Qty.Value
                            };
                            _context.Add(inventoryBin);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            inventoryBin.Qty = inventoryBin.Qty + model.Qty.Value;
                            await _context.SaveChangesAsync();
                        }

                        // sum all bins of selected zoneId
                        var currentZone = await _context.Bins
                            .AsNoTracking()
                            .Select(x => new
                            {
                                x.BinId,
                                x.ZoneId
                            })
                            .SingleOrDefaultAsync(x => x.BinId == binDb.BinId);

                        var inventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == itemId
                                && x.Bin.ZoneId == currentZone.ZoneId)
                            .SumAsync(x => x.Qty);

                        var inventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.ZoneId == currentZone.ZoneId);

                        if (inventoryZone is null)
                        {
                            inventoryZone = new InventoryZone
                            {
                                CustomerLocationId = model.CustomerLocationId,
                                InventoryId = inventory.InventoryId,
                                ZoneId = currentZone.ZoneId,
                                Qty = inventoryBinQty
                            };
                            _context.Add(inventoryZone);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            inventoryZone.Qty = inventoryBinQty;
                            await _context.SaveChangesAsync();
                        }

                        if (inventoryExist)
                        {
                            // sum all zones of selected inventory
                            var inventoryZoneQty = await _context.InventoryZones
                                .Include(x => x.Inventory)
                                .Where(x => x.Inventory.ItemId == itemId)
                                .SumAsync(x => x.Qty);

                            // sum selected inventory in pallet
                            var inventoryPalletQty = await _context.PalletInventories
                                .Include(x => x.Inventory)
                                .Where(x => x.Inventory.ItemId == itemId)
                                .SumAsync(x => x.Qty);

                            inventoryQty = inventoryZoneQty + inventoryPalletQty;

                            inventory.QtyOnHand = inventoryQty;

                            await _context.SaveChangesAsync();
                        }

                        // create log
                        var activityLog = new ActivityLog
                        {
                            CustomerId = customerId,
                            Type = putAway.PutAwayType == PutAwayTypeEnum.Receive ? ActivityLogTypeEnum.PurchaseOrder
                                : ActivityLogTypeEnum.Receipt,
                            InventoryId = inventory.InventoryId,
                            UserId = model.UserId,
                            ActivityDateTime = DateTime.Now,
                            Qty = model.Qty.Value,
                            OldQty = oldQty,
                            NewQty = inventoryQty,
                            MathematicalSymbol = MathematicalSymbolEnum.Plus,
                            ZoneId = binDb.ZoneId,
                            BinId = binDb.BinId
                        };
                        _context.Add(activityLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new PutAwayBinGetModel
                        {
                            PutAwayBinId = entity.PutAwayBinId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            PutAwayId = entity.PutAwayId.Value,
                            BinId = entity.BinId.Value,
                            Qty = entity.Qty,
                            ReceivedDateTime = entity.ReceivedDateTime
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

        public async Task<Result<PutAwayGetModel>> CreatePutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.ReceiveId is null)
                {
                    return Result.Fail($"{nameof(PutAwayCreateModel.ReceiveId)} is required.");
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(PutAwayCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(PutAwayCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(PutAwayCreateModel.Qty)} cannot be zero.");
                    }
                }

                var receive = await _context.Receives
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.ReceiveId,
                        x.CustomerLocationId,
                        x.Remaining
                    })
                    .SingleOrDefaultAsync(x => x.ReceiveId == model.ReceiveId
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (receive is null)
                {
                    return Result.Fail($"{nameof(Receive)} not found.");
                }

                if (model.Qty > receive.Remaining)
                {
                    return Result.Fail($"Item only have a remaining quantity of: {receive.Remaining}. Cannot receive more than the item remaining quantity.");
                }

                var putAways = await _context.PutAways
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.ReceiveId,
                        x.Remaining
                    })
                    .Where(x => x.ReceiveId == receive.ReceiveId)
                    .ToListAsync();

                var remaining = receive.Remaining;
                if (putAways is not null && putAways.Count > 0)
                {
                    var putAwayRemaining = putAways.Sum(x => x.Remaining);
                    remaining = receive.Remaining - putAwayRemaining;

                    if (model.Qty > receive.Remaining - putAwayRemaining)
                    {
                        return Result.Fail($"Item have a pending quantity of: {putAwayRemaining} at the put away queue. You only have a remaining quantity of {remaining} that can be added at put away queue.");
                    }
                }

                var entity = new PutAway
                {
                    CustomerLocationId = state.CustomerLocationId,
                    ReceiveId = model.ReceiveId,
                    Qty = model.Qty.Value,
                    Remaining = model.Qty.Value,
                    PutAwayType = PutAwayTypeEnum.Receive,
                    PutAwayDate = DateTime.Now
                };
                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new PutAwayGetModel
                {
                    PutAwayId = entity.PutAwayId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    ReceiveId = entity.ReceiveId.Value,
                    Qty = entity.Qty,
                    Remaining = remaining - model.Qty.Value,
                    PutAwayDate = entity.PutAwayDate
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<PutAwayBinGetModel>> CreatePutAwayBinDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayBinDeviceCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinDeviceCreateModel)} is required.");
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
                    return Result.Fail($"{nameof(PutAwayBinDeviceCreateModel.UserId)} is required.");
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

                if (model.PutAwayId is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinDeviceCreateModel.PutAwayId)} is required.");
                }
                else
                {
                    var exist = await _context.PutAways
                        .AnyAsync(x => x.PutAwayId == model.PutAwayId
                            && x.CustomerLocationId == state.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(PutAway)} not found.");
                    }
                }

                if (model.BinGetCreate is null)
                {
                    return Result.Fail($"{nameof(model.BinGetCreate)} is required.");
                }

                if (model.BinGetCreate.ZoneId is null)
                {
                    return Result.Fail($"{nameof(model.BinGetCreate)}.{nameof(model.BinGetCreate.ZoneId)} is required.");
                }

                if (!model.BinGetCreate.Name.HasValue())
                {
                    return Result.Fail($"{nameof(model.BinGetCreate)}.{nameof(model.BinGetCreate.Name)} is required.");
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinDeviceCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(PutAwayBinDeviceCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(PutAwayBinDeviceCreateModel.Qty)} cannot be zero.");
                    }
                }

                if (model.PutAwayType is null)
                {
                    return Result.Fail($"{nameof(PutAwayBinDeviceCreateModel.PutAwayType)} is required.");
                }

                var putAway = await _context.PutAways
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.PutAwayId,
                        x.CustomerLocationId,
                        x.Remaining,
                        x.PutAwayType,
                        x.ReceiveId,
                        x.ReceiptId
                    })
                    .SingleOrDefaultAsync(x => x.PutAwayId == model.PutAwayId
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (model.Qty > putAway.Remaining)
                {
                    return Result.Fail($"Item's quantity to put away have remaining of: {putAway.Remaining}. Cannot put away more than that.");
                }

                int? lotId = null;
                if (putAway.PutAwayType == PutAwayTypeEnum.Receive)
                {
                    var receive = await _context.Receives
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ReceiveId == putAway.ReceiveId);

                    if (receive is null)
                    {
                        return Result.Fail("Receive not found.");
                    }

                    lotId = receive.LotId;

                    if (receive.LotId is not null)
                    {
                        var exist = await _context.Lots
                            .AnyAsync(x => x.LotId == receive.LotId
                                && x.ItemId == receive.ItemId);

                        if (!exist)
                        {
                            return Result.Fail("Lot not found.");
                        }
                    }
                }
                else
                {
                    var receipt = await _context.Receipts
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ReceiptId == putAway.ReceiptId);

                    if (receipt is null)
                    {
                        return Result.Fail("Receipt not found.");
                    }

                    lotId = receipt.LotId;

                    if (receipt.LotId is not null)
                    {
                        var exist = await _context.Lots
                            .AnyAsync(x => x.LotId == receipt.LotId
                                && x.ItemId == receipt.ItemId);

                        if (!exist)
                        {
                            return Result.Fail("Lot not found.");
                        }
                    }
                }

                var binCheck = await _context.Bins
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ZoneId == model.BinGetCreate.ZoneId
                        && x.Name.Trim() == model.BinGetCreate.Name.Trim()
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (binCheck is not null)
                {
                    var palletCheck = await _context.Pallets
                        .AsNoTracking()
                        .AnyAsync(x => x.BinId == binCheck.BinId);

                    if (palletCheck)
                    {
                        return Result.Fail($"Cannot put item in this location. 'Each' and 'Pallet' cannot be in same location.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Bin binDb = await _context.Bins
                            .SingleOrDefaultAsync(x => x.ZoneId == model.BinGetCreate.ZoneId
                                && x.Name.Trim() == model.BinGetCreate.Name.Trim()
                                && x.CustomerLocationId == state.CustomerLocationId);
                        if (binDb is null)
                        {
                            binDb = new Bin
                            {
                                CustomerLocationId = state.CustomerLocationId,
                                ZoneId = model.BinGetCreate.ZoneId,
                                Name = model.BinGetCreate.Name
                            };
                            _context.Add(binDb);
                            await _context.SaveChangesAsync();
                        }

                        var entity = new PutAwayBin
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            PutAwayId = model.PutAwayId,
                            BinId = binDb.BinId,
                            Qty = model.Qty.Value,
                            ReceivedDateTime = DateTime.Now
                        };
                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var selectedPutAwayBinQty = await _context.PutAwayBins
                            .Where(x => x.PutAwayId == model.PutAwayId)
                            .SumAsync(x => x.Qty);

                        // update current putaway remaining
                        var putAwayDb = await _context.PutAways
                            .SingleOrDefaultAsync(x => x.PutAwayId == model.PutAwayId);
                        putAwayDb.Remaining = putAwayDb.Qty - selectedPutAwayBinQty;
                        await _context.SaveChangesAsync();

                        // update receive remaining and received
                        var putAwayBinQty = await _context.PutAwayBins
                            .Include(x => x.PutAway)
                            .Where(x => model.PutAwayType == PutAwayTypeEnum.Receive ? x.PutAway.ReceiveId == putAwayDb.ReceiveId
                                : x.PutAway.ReceiptId == putAwayDb.ReceiptId)
                            .SumAsync(x => x.Qty);

                        int itemId;
                        if (model.PutAwayType == PutAwayTypeEnum.Receive)
                        {
                            var receive = await _context.Receives
                                .SingleOrDefaultAsync(x => x.ReceiveId == putAwayDb.ReceiveId);
                            receive.Remaining = receive.Qty - putAwayBinQty;
                            receive.Received = putAwayBinQty;
                            await _context.SaveChangesAsync();

                            // update purchase order remaining
                            var receiveRemaining = await _context.Receives
                                .Where(x => x.PurchaseOrderId == receive.PurchaseOrderId)
                                .SumAsync(x => x.Remaining);

                            var purchaseOrder = await _context.PurchaseOrders
                                .SingleOrDefaultAsync(x => x.PurchaseOrderId == receive.PurchaseOrderId);

                            purchaseOrder.Remaining = receiveRemaining;

                            if (purchaseOrder.Remaining == 0)
                            {
                                purchaseOrder.Status = PurchaseOrderStatusEnum.Closed;
                                purchaseOrder.UpdatedDateTime = DateTime.Now;
                            }

                            await _context.SaveChangesAsync();

                            itemId = receive.ItemId.Value;
                        }
                        else
                        {
                            // update current receipt remaining
                            var receiptDb = await _context.Receipts
                                .SingleOrDefaultAsync(x => x.ReceiptId == putAwayDb.ReceiptId);
                            receiptDb.Remaining = receiptDb.Qty - putAwayBinQty;
                            await _context.SaveChangesAsync();

                            itemId = receiptDb.ItemId.Value;
                        }

                        // update inventory qty, zone qty and bin qty
                        var inventoryExist = false;
                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == itemId);

                        var oldQty = 0;
                        var inventoryQty = 0;

                        if (inventory is null) // create
                        {
                            inventory = new Inventory
                            {
                                CustomerId = state.CustomerId,
                                ItemId = itemId,
                                QtyOnHand = model.Qty.Value
                            };
                            _context.Add(inventory);
                            await _context.SaveChangesAsync();

                            oldQty = model.Qty.Value;
                            inventoryQty = model.Qty.Value;
                        }
                        else
                        {
                            inventoryExist = true;
                            oldQty = inventory.QtyOnHand;
                        }

                        var inventoryBin = await _context.InventoryBins
                            .Include(x => x.Lot)
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.BinId == binDb.BinId
                                && x.CustomerLocationId == state.CustomerLocationId);

                        if (inventoryBin is null)
                        {
                            inventoryBin = new InventoryBin
                            {
                                CustomerLocationId = state.CustomerLocationId,
                                InventoryId = inventory.InventoryId,
                                BinId = binDb.BinId,
                                Qty = model.Qty.Value,
                                LotId = lotId
                            };
                            _context.Add(inventoryBin);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            if (inventoryBin.LotId != lotId)
                            {
                                await transaction.RollbackAsync();

                                if (inventoryBin.LotId is not null)
                                {
                                    return Result.Fail($"Cannot put item with different lot number in the same location. Item's lot number in this location: {inventoryBin.Lot.LotNo}.");
                                }
                                else
                                {
                                    return Result.Fail($"Cannot put item with different lot number in the same location. Item's lot number in this location: NO LOT NUMBER.");
                                }
                            }

                            inventoryBin.Qty = inventoryBin.Qty + model.Qty.Value;
                            await _context.SaveChangesAsync();
                        }

                        // sum all bins of selected zoneId
                        var currentZone = await _context.Bins
                            .AsNoTracking()
                            .Select(x => new
                            {
                                x.BinId,
                                x.ZoneId
                            })
                            .SingleOrDefaultAsync(x => x.BinId == binDb.BinId);

                        var inventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == itemId
                                && x.Bin.ZoneId == currentZone.ZoneId)
                            .SumAsync(x => x.Qty);

                        var inventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.ZoneId == currentZone.ZoneId);

                        if (inventoryZone is null)
                        {
                            inventoryZone = new InventoryZone
                            {
                                CustomerLocationId = state.CustomerLocationId,
                                InventoryId = inventory.InventoryId,
                                ZoneId = currentZone.ZoneId,
                                Qty = inventoryBinQty
                            };
                            _context.Add(inventoryZone);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            inventoryZone.Qty = inventoryBinQty;
                            await _context.SaveChangesAsync();
                        }

                        if (inventoryExist)
                        {
                            // sum all zones of selected inventory
                            var inventoryZoneQty = await _context.InventoryZones
                                .Include(x => x.Inventory)
                                .Where(x => x.Inventory.ItemId == itemId)
                                .SumAsync(x => x.Qty);

                            // sum selected inventory in pallet
                            var inventoryPalletQty = await _context.PalletInventories
                                .Include(x => x.Inventory)
                                .Where(x => x.Inventory.ItemId == itemId)
                                .SumAsync(x => x.Qty);

                            inventoryQty = inventoryZoneQty + inventoryPalletQty;

                            inventory.QtyOnHand = inventoryQty;

                            await _context.SaveChangesAsync();
                        }

                        // create log
                        var activityLog = new ActivityLog
                        {
                            CustomerId = state.CustomerId,
                            Type = model.PutAwayType == PutAwayTypeEnum.Receive ? ActivityLogTypeEnum.PurchaseOrder
                                : ActivityLogTypeEnum.Receipt,
                            InventoryId = inventory.InventoryId,
                            UserId = model.UserId,
                            ActivityDateTime = DateTime.Now,
                            Qty = model.Qty.Value,
                            OldQty = oldQty,
                            NewQty = inventoryQty,
                            MathematicalSymbol = MathematicalSymbolEnum.Plus,
                            ZoneId = binDb.ZoneId,
                            BinId = binDb.BinId
                        };
                        _context.Add(activityLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new PutAwayBinGetModel
                        {
                            PutAwayBinId = entity.PutAwayBinId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            PutAwayId = entity.PutAwayId.Value,
                            BinId = entity.BinId.Value,
                            Qty = entity.Qty,
                            ReceivedDateTime = entity.ReceivedDateTime
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

        public async Task<Result<IEnumerable<PutAwayLookupDeviceGetModel>>> GetPutAwayLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (skuSearch && !searchText.HasValue())
                {
                    return Result.Fail("Item SKU is required.");
                }

                var query = _context.PutAways
                    .Include(x => x.Receive)
                        .ThenInclude(x => x.Item)
                    .Include(x => x.Receipt)
                        .ThenInclude(x => x.Item)
                    .Include(x => x.Receive)
                        .ThenInclude(x => x.Lot)
                    .Include(x => x.Receipt)
                        .ThenInclude(x => x.Lot)
                    .Include(x => x.CustomerLocation)
                        .ThenInclude(x => x.CustomerFacilities)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (skuSearch)
                    {
                        query = query.Where(x => x.Remaining > 0
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                            && x.Receive.Item.SKU.Trim() == searchText.Trim()
                            && (x.PutAwayType == PutAwayTypeEnum.Receive || x.PutAwayType == PutAwayTypeEnum.Receipt));
                    }
                    else
                    {
                        query = query.Where(x => x.Remaining > 0
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                            && (x.PutAwayType == PutAwayTypeEnum.Receive || x.PutAwayType == PutAwayTypeEnum.Receipt)
                            && (x.Receive.Item.SKU.Trim().ToLower().Contains(searchText)
                                || x.Receive.Item.Description.Trim().ToLower().Contains(searchText)
                                || x.Remaining.ToString().Trim().ToLower().Contains(searchText)));
                    }
                }
                else
                {
                    query = query.Where(x => x.Remaining > 0
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                        && (x.PutAwayType == PutAwayTypeEnum.Receive || x.PutAwayType == PutAwayTypeEnum.Receipt));
                }

                IEnumerable<PutAwayLookupDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new PutAwayLookupDeviceGetModel
                    {
                        PutAwayId = x.PutAwayId,
                        ItemId = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.ItemId : x.Receipt.Item.ItemId,
                        SKU = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.SKU : x.Receipt.Item.SKU,
                        Description = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.Description : x.Receipt.Item.Description,
                        Qty = x.Qty,
                        Remaining = x.Remaining,
                        PutAwayType = x.PutAwayType,
                        LotNo = x.PutAwayType == PutAwayTypeEnum.Receive
                            ? (x.Receive.Lot != null ? x.Receive.Lot.LotNo : null)
                            : (x.Receipt.Lot != null ? x.Receipt.Lot.LotNo : null),
                        ExpirationDate = x.PutAwayType == PutAwayTypeEnum.Receive
                            ? (x.Receive.Lot != null ? x.Receive.Lot.ExpirationDate : null)
                            : (x.Receipt.Lot != null ? x.Receipt.Lot.ExpirationDate : null),
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

        public async Task<Result<PutAwayLookupDeviceGetModel>> GetPutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, int putAwayId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var model = await _context.PutAways
                    .Include(x => x.Receive)
                        .ThenInclude(x => x.Item)
                    .Include(x => x.Receipt)
                        .ThenInclude(x => x.Item)
                    .AsNoTracking()
                    .Where(x => x.PutAwayId == putAwayId
                        && x.CustomerLocationId == state.CustomerLocationId)
                    .Select(x => new PutAwayLookupDeviceGetModel
                    {
                        PutAwayId = x.PutAwayId,
                        SKU = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.SKU : x.Receipt.Item.SKU,
                        Description = x.PutAwayType == PutAwayTypeEnum.Receive ? x.Receive.Item.Description : x.Receipt.Item.Description,
                        Qty = x.Qty,
                        Remaining = x.Remaining,
                        PutAwayType = x.PutAwayType
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

        public async Task<Result<IEnumerable<PutAwayLookupLicensePlateDeviceGetModel>>> GetPutAwayLookupLicensePlateDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (barcodeSearch && !searchText.HasValue())
                {
                    return Result.Fail("License plate is required.");
                }

                var query = _context.PutAways
                    .Include(x => x.LicensePlate)
                        .ThenInclude(x => x.LicensePlateItems)
                            .ThenInclude(x => x.Item)
                    .Include(x => x.CustomerLocation)
                        .ThenInclude(x => x.CustomerFacilities)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (barcodeSearch)
                    {
                        query = query.Where(x => x.Remaining > 0
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                            && x.LicensePlate.LicensePlateNo.Trim() == searchText.Trim()
                            && x.PutAwayType == PutAwayTypeEnum.LicensePlate);
                    }
                    else
                    {
                        query = query.Where(x => x.Remaining > 0
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                            && x.PutAwayType == PutAwayTypeEnum.LicensePlate
                            && (x.LicensePlate.LicensePlateNo.Trim().ToLower().Contains(searchText)
                                || x.Remaining.ToString().Trim().ToLower().Contains(searchText)));
                    }
                }
                else
                {
                    query = query.Where(x => x.Remaining > 0
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                        && x.PutAwayType == PutAwayTypeEnum.LicensePlate);
                }

                IEnumerable<PutAwayLookupLicensePlateDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new PutAwayLookupLicensePlateDeviceGetModel
                    {
                        PutAwayId = x.PutAwayId,
                        LicensePlateId = x.LicensePlate.LicensePlateId,
                        LicensePlateNo = x.LicensePlate.LicensePlateNo,
                        LicensePlateType = x.LicensePlate.LicensePlateType,
                        Items = x.LicensePlate.LicensePlateItems.Select(z => new PutAwayLookupLicensePlateDeviceGetModel.Item
                        {
                            ItemId = z.Item.ItemId,
                            ItemSKU = z.Item.SKU,
                            ItemDescription = z.Item.Description,
                            TotalQty = z.TotalQty
                        })
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlate)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<PutAwayBinGetModel>>> CreatePutAwayPalletDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayPalletDeviceCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(PutAwayPalletDeviceCreateModel)} is required.");
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

                if (model.CustomerFacilityId is null)
                {
                    return Result.Fail($"{nameof(PutAwayPalletDeviceCreateModel.CustomerFacilityId)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerFacilities
                       .AnyAsync(x => x.CustomerLocationId == state.CustomerLocationId
                           && x.CustomerFacilityId == model.CustomerFacilityId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerFacility)} not found.");
                    }
                }

                if (model.UserId is null)
                {
                    return Result.Fail($"{nameof(PutAwayPalletDeviceCreateModel.UserId)} is required.");
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

                if (model.BinGetCreate is null)
                {
                    return Result.Fail($"{nameof(model.BinGetCreate)} is required.");
                }

                if (model.BinGetCreate.ZoneId is null)
                {
                    return Result.Fail($"{nameof(model.BinGetCreate)}.{nameof(model.BinGetCreate.ZoneId)} is required.");
                }

                if (!model.BinGetCreate.Name.HasValue())
                {
                    return Result.Fail($"{nameof(model.BinGetCreate)}.{nameof(model.BinGetCreate.Name)} is required.");
                }

                var pas = new List<PutAway>();

                if (model.PutAways is null || model.PutAways.Count() == 0)
                {
                    return Result.Fail($"{nameof(model.PutAways)} is required.");
                }
                else
                {
                    if (model.PutAways.Any(x => x.PutAwayId is null))
                    {
                        return Result.Fail($"Put away is required.");
                    }
                    else
                    {
                        foreach (var x in model.PutAways)
                        {
                            var pa = await _context.PutAways
                                .AsNoTracking()
                                .SingleOrDefaultAsync(z => z.PutAwayId == x.PutAwayId
                                    && z.CustomerLocationId == state.CustomerLocationId
                                    && z.PutAwayType == PutAwayTypeEnum.LicensePlate
                                    && z.Remaining > 0);

                            if (pa is null)
                            {
                                return Result.Fail($"{nameof(PutAway)} not found.");
                            }
                            else
                            {
                                pas.Add(pa);
                            }

                            var lpCheck = await _context.LicensePlates
                                .Include(x => x.LicensePlateItems)
                                    .ThenInclude(x => x.Item)
                                        .ThenInclude(x => x.Inventory)
                                            .ThenInclude(x => x.InventoryBins)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(x => x.LicensePlateId == pa.LicensePlateId
                                    && x.PalletId == null);

                            if (lpCheck is null)
                            {
                                return Result.Fail($"{nameof(LicensePlate)} not found.");
                            }

                            var binCheck = await _context.Bins
                                .AsNoTracking()
                                .SingleOrDefaultAsync(x => x.ZoneId == model.BinGetCreate.ZoneId
                                    && x.Name.Trim() == model.BinGetCreate.Name.Trim()
                                    && x.CustomerLocationId == state.CustomerLocationId);

                            if (binCheck is not null)
                            {
                                foreach (var z in lpCheck.LicensePlateItems)
                                {
                                    if (z.Item.Inventory is not null)
                                    {
                                        if (z.Item.Inventory.InventoryBins.Any(z => z.BinId == binCheck.BinId && z.Qty > 0))
                                        {
                                            return Result.Fail($"Cannot put pallet in location because this location have 'Eaches' of item: {z.Item.SKU}, {z.Item.Description}.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Bin binDb = await _context.Bins
                            .SingleOrDefaultAsync(x => x.ZoneId == model.BinGetCreate.ZoneId
                                && x.Name.Trim() == model.BinGetCreate.Name.Trim()
                                && x.CustomerLocationId == state.CustomerLocationId);
                        if (binDb is null)
                        {
                            binDb = new Bin
                            {
                                CustomerLocationId = state.CustomerLocationId,
                                ZoneId = model.BinGetCreate.ZoneId,
                                Name = model.BinGetCreate.Name
                            };
                            _context.Add(binDb);
                            await _context.SaveChangesAsync();
                        }

                        var models = new List<PutAwayBinGetModel>();
                        var totalQty = 0;
                        foreach (var x in model.PutAways)
                        {
                            var qty = pas.SingleOrDefault(z => z.PutAwayId == x.PutAwayId).Qty;
                            totalQty += qty;

                            var entity = new PutAwayBin
                            {
                                CustomerLocationId = state.CustomerLocationId,
                                PutAwayId = x.PutAwayId,
                                BinId = binDb.BinId,
                                Qty = qty,
                                ReceivedDateTime = DateTime.Now
                            };
                            _context.Add(entity);
                            await _context.SaveChangesAsync();

                            models.Add(new PutAwayBinGetModel
                            {
                                PutAwayBinId = entity.PutAwayBinId,
                                CustomerLocationId = entity.CustomerLocationId.Value,
                                PutAwayId = entity.PutAwayId.Value,
                                BinId = entity.BinId.Value,
                                Qty = entity.Qty,
                                ReceivedDateTime = entity.ReceivedDateTime
                            });

                            // update current putaway remaining
                            var putAwayDb = await _context.PutAways
                                .SingleOrDefaultAsync(z => z.PutAwayId == x.PutAwayId);
                            putAwayDb.Remaining = 0;
                            await _context.SaveChangesAsync();
                        }

                        // create pallet
                        var pallet = new Pallet
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            CustomerFacilityId = model.CustomerFacilityId,
                            BinId = binDb.BinId,
                            Qty = totalQty,
                            CreatedDateTime = DateTime.Now,
                            MixedPallet = pas.Count > 1 ? true : false
                        };
                        _context.Add(pallet);
                        await _context.SaveChangesAsync();

                        foreach (var x in pas)
                        {
                            var lpDb = await _context.LicensePlates
                                .Include(x => x.LicensePlateItems)
                                    .ThenInclude(x => x.Item)
                                .SingleOrDefaultAsync(z => z.LicensePlateId == x.LicensePlateId);

                            if (lpDb is not null)
                            {
                                lpDb.PalletId = pallet.PalletId;
                                await _context.SaveChangesAsync();

                                foreach (var z in lpDb.LicensePlateItems)
                                {
                                    // update inventory qty
                                    var itemId = z.Item.ItemId;
                                    var inventoryExist = false;
                                    var inventory = await _context.Inventories
                                        .SingleOrDefaultAsync(a => a.ItemId == itemId);

                                    var oldQty = 0;
                                    var inventoryQty = 0;

                                    if (inventory is null) // create
                                    {
                                        inventory = new Inventory
                                        {
                                            CustomerId = state.CustomerId,
                                            ItemId = itemId,
                                            QtyOnHand = z.TotalQty // no need to sum, because it is new item
                                        };
                                        _context.Add(inventory);
                                        await _context.SaveChangesAsync();

                                        oldQty = z.TotalQty;
                                        inventoryQty = z.TotalQty;
                                    }
                                    else
                                    {
                                        inventoryExist = true;
                                        oldQty = inventory.QtyOnHand;
                                    }

                                    // create pallet inventory
                                    var pi = new PalletInventory
                                    {
                                        CustomerLocationId = state.CustomerLocationId,
                                        CustomerFacilityId = model.CustomerFacilityId,
                                        PalletId = pallet.PalletId,
                                        InventoryId = inventory.InventoryId,
                                        LicensePlateItemId = z.LicensePlateItemId,
                                        Qty = z.TotalQty
                                    };
                                    _context.Add(pi);
                                    await _context.SaveChangesAsync();

                                    if (inventoryExist)
                                    {
                                        // sum all zones of selected inventory
                                        var inventoryZoneQty = await _context.InventoryZones
                                            .Include(x => x.Inventory)
                                            .Where(x => x.Inventory.ItemId == itemId)
                                            .SumAsync(x => x.Qty);

                                        // sum selected inventory in pallet
                                        var inventoryPalletQty = await _context.PalletInventories
                                            .Include(x => x.Inventory)
                                            .Where(x => x.Inventory.ItemId == itemId)
                                            .SumAsync(x => x.Qty);

                                        inventoryQty = inventoryZoneQty + inventoryPalletQty;

                                        inventory.QtyOnHand = inventoryQty;

                                        await _context.SaveChangesAsync();
                                    }

                                    // create log
                                    var activityLog = new ActivityLog
                                    {
                                        CustomerId = state.CustomerId,
                                        Type = ActivityLogTypeEnum.Palletize,
                                        InventoryId = inventory.InventoryId,
                                        UserId = model.UserId,
                                        ActivityDateTime = DateTime.Now,
                                        Qty = z.TotalQty,
                                        OldQty = oldQty,
                                        NewQty = inventoryQty,
                                        MathematicalSymbol = MathematicalSymbolEnum.Plus,
                                        ZoneId = binDb.ZoneId,
                                        BinId = binDb.BinId
                                    };
                                    _context.Add(activityLog);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                        await transaction.CommitAsync();

                        return Result.Ok(models.AsEnumerable());
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