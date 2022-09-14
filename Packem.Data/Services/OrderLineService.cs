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
    public class OrderLineService : IOrderLineService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public OrderLineService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<OrderLineGetModel>> CreateOrderLineAsync(AppState state, OrderLineCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                SaleOrder so;
                if (model.SaleOrderId is null)
                {
                    return Result.Fail($"{nameof(OrderLineCreateModel.SaleOrderId)} is required.");
                }
                else
                {
                    so = await _context.SaleOrders
                        .Include(x => x.CustomerLocation)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.SaleOrderId == model.SaleOrderId);

                    if (so is null)
                    {
                        return Result.Fail($"{nameof(SaleOrder)} not found.");
                    }
                }

                Item item;
                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(OrderLineCreateModel.ItemId)} is required.");
                }
                else
                {
                    item = await _context.Items
                        .Include(x => x.Inventory)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ItemId == model.ItemId
                            && x.CustomerId == so.CustomerLocation.CustomerId);

                    if (item is null)
                    {
                        return Result.Fail($"{nameof(Item)} not found.");
                    }
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(OrderLineCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(OrderLineCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(OrderLineCreateModel.Qty)} cannot be zero.");
                    }
                }

                if (model.Qty > item.Inventory.QtyOnHand)
                {
                    return Result.Fail($"Cannot order more than the total quantity of item: {item.Inventory.QtyOnHand}.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = new OrderLine
                        {
                            CustomerLocationId = so.CustomerLocationId,
                            SaleOrderId = model.SaleOrderId,
                            ItemId = model.ItemId,
                            Qty = model.Qty.Value,
                            Received = 0,
                            Remaining = model.Qty.Value,
                            PerUnitPrice = model.PerUnitItemPrice,
                        };

                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        // update so qty
                        var soDb = await _context.SaleOrders
                            .SingleOrDefaultAsync(x => x.SaleOrderId == model.SaleOrderId);

                        // get all ol by SaleOrderId
                        var olQty = await _context.OrderLines
                            .Where(x => x.SaleOrderId == model.SaleOrderId)
                            .SumAsync(x => x.Qty);

                        var olRemaining = await _context.OrderLines
                            .Where(x => x.SaleOrderId == model.SaleOrderId)
                            .SumAsync(x => x.Remaining);

                        soDb.OrderQty = olQty;
                        soDb.Remaining = olRemaining;
                        await _context.SaveChangesAsync();

                        // update so totalSalePrice
                        var ols = await _context.OrderLines
                            .Where(x => x.SaleOrderId == model.SaleOrderId)
                            .AsNoTracking()
                            .ToListAsync();

                        decimal totalSalePrice = 0;
                        foreach (var x in ols)
                        {
                            if (x.PerUnitPrice is not null)
                            {
                                totalSalePrice += x.Qty * x.PerUnitPrice.Value;
                            }
                        }
                        soDb.TotalSalePrice = totalSalePrice;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new OrderLineGetModel
                        {
                            OrderLineId = entity.OrderLineId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            SaleOrderId = entity.SaleOrderId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Received = entity.Received,
                            Remaining = entity.Remaining,
                            PerUnitItemPrice = entity.PerUnitPrice,
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

        public async Task<Result<OrderLineGetModel>> UpdateOrderLineAsync(AppState state, OrderLineUpdateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                OrderLine ol;
                if (model.OrderLineId is null)
                {
                    return Result.Fail($"{nameof(OrderLineUpdateModel.OrderLineId)} is required.");
                }
                else
                {
                    ol = await _context.OrderLines
                        .Include(x => x.CustomerLocation)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.OrderLineId == model.OrderLineId);

                    if (ol is null)
                    {
                        return Result.Fail($"{nameof(OrderLine)} not found.");
                    }
                }

                var so = await _context.SaleOrders
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.SaleOrderId == ol.SaleOrderId
                        && x.PickingStatus != PickingStatusEnum.Complete);

                if (so is null)
                {
                    return Result.Fail($"{nameof(SaleOrder)} not found.");
                }

                var item = await _context.Items
                    .Include(x => x.Inventory)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ItemId == ol.ItemId
                        && x.CustomerId == ol.CustomerLocation.CustomerId);

                if (item is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(OrderLineUpdateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(OrderLineUpdateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(OrderLineUpdateModel.Qty)} cannot be zero.");
                    }
                }

                if (model.Qty > item.Inventory.QtyOnHand)
                {
                    return Result.Fail($"Cannot order more than the total quantity of item: {item.Inventory.QtyOnHand}.");
                }

                if (ol.Received > model.Qty.Value)
                {
                    return Result.Fail($"Cannot adjust the quantity below the received quantity: {ol.Received}.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.OrderLines
                            .SingleOrDefaultAsync(x => x.OrderLineId == model.OrderLineId);

                        entity.Remaining = model.Qty.Value - entity.Received;
                        entity.Qty = model.Qty.Value;
                        entity.PerUnitPrice = model.PerUnitPrice;
                        await _context.SaveChangesAsync();

                        // update so qty
                        var soDb = await _context.SaleOrders
                            .SingleOrDefaultAsync(x => x.SaleOrderId == ol.SaleOrderId);

                        // get all ol by SaleOrderId
                        var olQty = await _context.OrderLines
                            .Where(x => x.SaleOrderId == ol.SaleOrderId)
                            .SumAsync(x => x.Qty);

                        var olRemaining = await _context.OrderLines
                            .Where(x => x.SaleOrderId == ol.SaleOrderId)
                            .SumAsync(x => x.Remaining);

                        soDb.OrderQty = olQty;
                        soDb.Remaining = olRemaining;
                        await _context.SaveChangesAsync();

                        // update so totalSalePrice
                        var ols = await _context.OrderLines
                            .Where(x => x.SaleOrderId == ol.SaleOrderId)
                            .AsNoTracking()
                            .ToListAsync();

                        decimal totalSalePrice = 0;
                        foreach (var x in ols)
                        {
                            if (x.PerUnitPrice is not null)
                            {
                                totalSalePrice += x.Qty * x.PerUnitPrice.Value;
                            }
                        }
                        soDb.TotalSalePrice = totalSalePrice;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new OrderLineGetModel
                        {
                            OrderLineId = entity.OrderLineId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            SaleOrderId = entity.SaleOrderId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Received = entity.Received,
                            Remaining = entity.Remaining,
                            PerUnitItemPrice = entity.PerUnitPrice,
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

        public async Task<Result<OrderLineGetModel>> DeleteOrderLineAsync(AppState state, OrderLineDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                OrderLine ol;
                if (model.OrderLineId is null)
                {
                    return Result.Fail($"{nameof(OrderLineUpdateModel.OrderLineId)} is required.");
                }
                else
                {
                    ol = await _context.OrderLines
                        .Include(x => x.CustomerLocation)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.OrderLineId == model.OrderLineId);

                    if (ol is null)
                    {
                        return Result.Fail($"{nameof(OrderLine)} not found.");
                    }
                }

                if (ol.Received > 0)
                {
                    return Result.Fail($"Cannot cancel order line item because it started picking.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.OrderLines
                            .SingleOrDefaultAsync(x => x.OrderLineId == model.OrderLineId);

                        entity.Deleted = true;
                        await _context.SaveChangesAsync();

                        // update so qty
                        var soDb = await _context.SaleOrders
                            .SingleOrDefaultAsync(x => x.SaleOrderId == ol.SaleOrderId);

                        // get all ol by SaleOrderId
                        var olQty = await _context.OrderLines
                            .Where(x => x.SaleOrderId == ol.SaleOrderId)
                            .SumAsync(x => x.Qty);

                        var olRemaining = await _context.OrderLines
                            .Where(x => x.SaleOrderId == ol.SaleOrderId)
                            .SumAsync(x => x.Remaining);

                        soDb.OrderQty = olQty;
                        soDb.Remaining = olRemaining;
                        await _context.SaveChangesAsync();

                        // update so totalSalePrice
                        var ols = await _context.OrderLines
                            .Where(x => x.SaleOrderId == ol.SaleOrderId)
                            .AsNoTracking()
                            .ToListAsync();

                        decimal totalSalePrice = 0;
                        foreach (var x in ols)
                        {
                            if (x.PerUnitPrice is not null)
                            {
                                totalSalePrice += x.Qty * x.PerUnitPrice.Value;
                            }
                        }
                        soDb.TotalSalePrice = totalSalePrice;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new OrderLineGetModel
                        {
                            OrderLineId = entity.OrderLineId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            SaleOrderId = entity.SaleOrderId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Received = entity.Received,
                            Remaining = entity.Remaining,
                            PerUnitItemPrice = entity.PerUnitPrice,
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

        public async Task<Result<OrderLineGetModel>> GetOrderLineDeviceAsync(CustomerDeviceTokenAuthModel state, int orderLineId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var model = await _context.OrderLines
                    .Where(x => x.OrderLineId == orderLineId)
                    .AsNoTracking()
                    .Select(x => new OrderLineGetModel
                    {
                        OrderLineId = x.OrderLineId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        SaleOrderId = x.SaleOrderId.Value,
                        ItemId = x.ItemId.Value,
                        Qty = x.Qty,
                        Received = x.Received,
                        Remaining = x.Remaining
                    })
                    .SingleOrDefaultAsync();

                if (model == null)
                {
                    return Result.Fail($"{nameof(OrderLine)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<OrderLinePickLookupGetModel>>> GetOrderLinePickLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int saleOrderId, string searchText, bool barcodeSearch = false)
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

                var query = _context.OrderLines
                    .Include(x => x.SaleOrder)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.InventoryBins)
                                .ThenInclude(x => x.Bin)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (barcodeSearch)
                    {
                        query = query.Where(x => x.SaleOrderId == saleOrderId
                            && x.SaleOrder.Status == SaleOrderStatusEnum.Printed
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.Item.SKU.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => x.SaleOrderId == saleOrderId
                            && x.SaleOrder.Status == SaleOrderStatusEnum.Printed
                            && x.CustomerLocationId == state.CustomerLocationId
                            && (x.Item.SKU.Trim().ToLower().Contains(searchText)
                                || x.Item.Description.Trim().ToLower().Contains(searchText)
                                || x.Item.Inventory.InventoryBins.Any(z => z.Bin.Name.Trim().ToLower().Contains(searchText))));
                    }
                }
                else
                {
                    query = query.Where(x => x.SaleOrderId == saleOrderId
                        && x.SaleOrder.Status == SaleOrderStatusEnum.Printed
                        && x.CustomerLocationId == state.CustomerLocationId);
                }

                IEnumerable<OrderLinePickLookupGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new OrderLinePickLookupGetModel
                    {
                        OrderLineId = x.OrderLineId,
                        ItemId = x.Item.ItemId,
                        ItemSKU = x.Item.SKU,
                        ItemDescription = x.Item.Description,
                        ItemUOM = x.Item.UnitOfMeasure.Code,
                        Qty = x.Qty,
                        Received = x.Received,
                        Remaining = x.Remaining,
                        Bins = x.Item.Inventory.InventoryBins.Select(z => new OrderLinePickLookupGetModel.Bin
                        {
                            BinId = z.Bin.BinId,
                            Name = z.Bin.Name
                        })
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(OrderLine)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<OrderLineBinGetModel>> CreateOrderLineBinDeviceAsync(CustomerDeviceTokenAuthModel state, OrderLineBinCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(OrderLineBinCreateModel)} is required.");
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

                if (model.OrderLineId is null)
                {
                    return Result.Fail($"{nameof(OrderLineBinCreateModel.OrderLineId)} is required.");
                }
                else
                {
                    var exist = await _context.OrderLines
                        .Include(x => x.SaleOrder)
                        .AnyAsync(x => x.OrderLineId == model.OrderLineId
                            && x.SaleOrder.Status == SaleOrderStatusEnum.Printed
                            && x.CustomerLocationId == state.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(OrderLine)} not found.");
                    }
                }

                if (model.BinId is null)
                {
                    return Result.Fail($"{nameof(model.BinId)} is required.");
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
                    return Result.Fail($"{nameof(OrderLineBinCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(OrderLineBinCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(OrderLineBinCreateModel.Qty)} cannot be zero.");
                    }
                }

                var orderLine = await _context.OrderLines
                    .Include(x => x.Item.Inventory)
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.OrderLineId,
                        x.CustomerLocationId,
                        x.Remaining,
                        x.ItemId
                    })
                    .SingleOrDefaultAsync(x => x.OrderLineId == model.OrderLineId
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (model.Qty > orderLine.Remaining)
                {
                    return Result.Fail($"Item's quantity to pick have remaining of: {orderLine.Remaining}. Cannot pick more than that.");
                }

                var bin = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BinId == model.BinId
                        && x.Inventory.ItemId == orderLine.ItemId
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (bin == null)
                {
                    return Result.Fail($"Item doesn't exist in this location");
                }
                else
                {
                    if (model.Qty > bin.Qty)
                    {
                        return Result.Fail($"Cannot pick more than the quantity of the item in the selected location which only have: {bin.Qty}.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = new OrderLineBin
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            OrderLineId = model.OrderLineId,
                            BinId = model.BinId,
                            Qty = model.Qty.Value,
                            PickDateTime = DateTime.Now
                        };
                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var selectedOrderLineBinQty = await _context.OrderLineBins
                            .Where(x => x.OrderLineId == model.OrderLineId)
                            .SumAsync(x => x.Qty);

                        // update current orderline remaining and received
                        var orderLineDb = await _context.OrderLines
                            .SingleOrDefaultAsync(x => x.OrderLineId == model.OrderLineId);
                        orderLineDb.Remaining = orderLineDb.Qty - selectedOrderLineBinQty;
                        orderLineDb.Received = selectedOrderLineBinQty;
                        await _context.SaveChangesAsync();

                        // update sale order remaining
                        var orderLineRemaining = await _context.OrderLines
                            .Where(x => x.SaleOrderId == orderLineDb.SaleOrderId)
                            .SumAsync(x => x.Remaining);

                        var saleOrder = await _context.SaleOrders
                            .SingleOrDefaultAsync(x => x.SaleOrderId == orderLineDb.SaleOrderId);

                        saleOrder.Remaining = orderLineRemaining;
                        await _context.SaveChangesAsync();

                        // decrease qty of current bin, zone and inventory
                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == orderLineDb.ItemId);

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
                            Type = ActivityLogTypeEnum.SaleOrder,
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

                        return Result.Ok(new OrderLineBinGetModel
                        {
                            OrderLineBinId = entity.OrderLineBinId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            OrderLineId = entity.OrderLineId.Value,
                            BinId = entity.BinId.Value,
                            Qty = entity.Qty,
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