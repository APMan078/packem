using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Entities;
using Packem.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class AdjustBinQuantityService : IAdjustBinQuantityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public AdjustBinQuantityService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<CreateAdjustBinQuantityGetModel>> GetCreateAdjustBinQuantityAsync(AppState state, int itemId, int customerLocationId, int binId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var item = await _context.Items
                    .Include(x => x.Inventory)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ItemId == itemId);

                if (item is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                var existCustomerLocation = await _context.CustomerLocations
                    .AnyAsync(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerId == item.CustomerId);

                if (!existCustomerLocation)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                var inventoryBin = await _context.InventoryBins
                    .Include(x => x.Bin)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.InventoryId == item.Inventory.InventoryId
                        && x.BinId == binId
                        && x.CustomerLocationId == customerLocationId);

                if (inventoryBin is null && inventoryBin.Bin is null)
                {
                    return Result.Fail($"Location not found.");
                }

                var model = new CreateAdjustBinQuantityGetModel();
                model.CustomerId = item.CustomerId.Value;
                model.CustomerLocationId = customerLocationId;
                model.ItemId = item.ItemId;
                model.BinId = binId;
                model.Message = $"Change known quantities for Item {item.SKU} in Location {inventoryBin.Bin.Name}.";

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<AdjustBinQuantityGetModel>> CreateAdjustBinQuantityAsync(AppState state, AdjustBinQuantityCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(AdjustBinQuantityCreateModel.CustomerId)} is required.");
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
                    return Result.Fail($"{nameof(AdjustBinQuantityCreateModel.CustomerLocationId)} is required.");
                }
                else
                {
                    var query = _context.CustomerLocations.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerLocationId == model.CustomerLocationId
                                && x.CustomerId == model.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerLocationId == model.CustomerLocationId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                var item = await _context.Items
                    .Include(x => x.Inventory)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ItemId == model.ItemId
                        && x.CustomerId == model.CustomerId);

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(AdjustBinQuantityCreateModel.ItemId)} is required.");
                }
                else
                {
                    if (item is null)
                    {
                        return Result.Fail("Item not found.");
                    }
                }

                if (model.NewQty is null)
                {
                    return Result.Fail($"{nameof(AdjustBinQuantityCreateModel.NewQty)} is required.");
                }
                else
                {
                    if (model.NewQty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(AdjustBinQuantityCreateModel.NewQty)} cannot be negative.");
                    }
                    else if (model.NewQty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(AdjustBinQuantityCreateModel.NewQty)} cannot be zero.");
                    }
                }

                InventoryBin inventoryBin;
                if (model.BinId is null)
                {
                    return Result.Fail($"{nameof(AdjustBinQuantityCreateModel.BinId)} is required.");
                }
                else
                {
                    inventoryBin = await _context.InventoryBins
                        .Include(x => x.Bin)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.InventoryId == item.Inventory.InventoryId
                            && x.BinId == model.BinId
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (inventoryBin is null && inventoryBin.Bin is null)
                    {
                        return Result.Fail($"{nameof(Bin)} not found.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var adjustBinQuantity = new AdjustBinQuantity
                        {
                            CustomerLocationId = model.CustomerLocationId,
                            ItemId = model.ItemId,
                            BinId = model.BinId,
                            OldQty = inventoryBin.Qty,
                            NewQty = model.NewQty.Value,
                            AdjustDateTime = DateTime.Now
                        };
                        _context.Add(adjustBinQuantity);
                        await _context.SaveChangesAsync();

                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == item.ItemId);

                        // update inventory qty, zone qty and bin qty
                        var inventoryBinDb = await _context.InventoryBins
                            .SingleOrDefaultAsync(x => x.InventoryBinId == inventoryBin.InventoryBinId);
                        inventoryBinDb.Qty = model.NewQty.Value;
                        await _context.SaveChangesAsync();

                        // sum all bins of selected zoneId
                        var inventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == item.ItemId
                                && x.Bin.ZoneId == inventoryBin.Bin.ZoneId)
                            .SumAsync(x => x.Qty);

                        var inventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.ZoneId == inventoryBin.Bin.ZoneId);

                        inventoryZone.Qty = inventoryBinQty;
                        await _context.SaveChangesAsync();

                        // sum all zones of selected inventory
                        var inventoryZoneQty = await _context.InventoryZones
                            .Where(x => x.InventoryId == inventory.InventoryId)
                            .SumAsync(x => x.Qty);

                        // sum selected inventory in pallet
                        var inventoryPalletQty = await _context.PalletInventories
                            .Where(x => x.InventoryId == inventory.InventoryId)
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
                            Type = ActivityLogTypeEnum.Adjustment,
                            InventoryId = inventory.InventoryId,
                            UserId = state.UserId,
                            ActivityDateTime = DateTime.Now,
                            Qty = model.NewQty.Value,
                            OldQty = inventoryBin.Qty,
                            NewQty = inventoryQty,
                            MathematicalSymbol = null,
                            ZoneId = currentBin.ZoneId,
                            BinId = currentBin.BinId
                        };
                        _context.Add(activityLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new AdjustBinQuantityGetModel
                        {
                            AdjustBinQuantityId = adjustBinQuantity.AdjustBinQuantityId,
                            CustomerLocationId = adjustBinQuantity.CustomerLocationId.Value,
                            ItemId = adjustBinQuantity.ItemId.Value,
                            BinId = adjustBinQuantity.BinId.Value,
                            OldQty = adjustBinQuantity.OldQty,
                            NewQty = adjustBinQuantity.NewQty,
                            AdjustDateTime = adjustBinQuantity.AdjustDateTime
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