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
    public class ReceiveService : IReceiveService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public ReceiveService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<ReceiveGetModel>> CreateReceiveAsync(AppState state, ReceiveCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(ReceiveCreateModel.CustomerId)} is required.");
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
                    return Result.Fail($"{nameof(ReceiveCreateModel.CustomerLocationId)} is required.");
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

                if (model.PurchaseOrderId is null)
                {
                    return Result.Fail($"{nameof(ReceiveCreateModel.PurchaseOrderId)} is required.");
                }
                else
                {
                    var exist = await _context.PurchaseOrders
                        .AnyAsync(x => x.PurchaseOrderId == model.PurchaseOrderId
                            && x.Status != PurchaseOrderStatusEnum.Closed
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(PurchaseOrder)} not found.");
                    }
                }

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(ReceiveCreateModel.ItemId)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.ItemId == model.ItemId
                            && x.CustomerId == model.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail("Item not found.");
                    }
                }

                //var existInPo = await _context.PurchaseOrders
                //    .Include(x => x.Receives)
                //    .AnyAsync(x => x.Receives.Any(z => z.ItemId == model.ItemId)
                //        && x.PurchaseOrderId == model.PurchaseOrderId
                //        && x.CustomerLocationId == model.CustomerLocationId);

                //if (existInPo)
                //{
                //    return Result.Fail($"{nameof(Item)} already exist in Purchase Order.");
                //}

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(ReceiveCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(ReceiveCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(ReceiveCreateModel.Qty)} cannot be zero.");
                    }
                }

                if (model.LotId is not null)
                {
                    var exist = await _context.Lots
                        .AnyAsync(x => x.LotId == model.LotId
                            && x.ItemId == model.ItemId);

                    if (!exist)
                    {
                        return Result.Fail("Lot not found.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var po = await _context.PurchaseOrders
                            .AsNoTracking()
                            .Select(x => new
                            {
                                x.PurchaseOrderId,
                                x.VendorId
                            })
                            .SingleOrDefaultAsync(x => x.PurchaseOrderId == model.PurchaseOrderId);

                        var itemVendor = await _context.ItemVendors
                            .SingleOrDefaultAsync(x => x.ItemId == model.ItemId
                                && x.VendorId == po.VendorId);

                        if (itemVendor is null)
                        {
                            itemVendor = new ItemVendor
                            {
                                CustomerId = model.CustomerId,
                                ItemId = model.ItemId,
                                VendorId = po.VendorId
                            };
                            _context.Add(itemVendor);
                            await _context.SaveChangesAsync();
                        }

                        var entity = new Receive
                        {
                            CustomerLocationId = model.CustomerLocationId,
                            PurchaseOrderId = model.PurchaseOrderId,
                            ItemId = model.ItemId,
                            Qty = model.Qty.Value,
                            Received = 0,
                            Remaining = model.Qty.Value,
                            LotId = model.LotId
                        };

                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var purchaseOrder = await _context.PurchaseOrders
                            .SingleOrDefaultAsync(x => x.PurchaseOrderId == model.PurchaseOrderId);

                        //if (purchaseOrder.Remaining == 0)
                        //{
                        //    return Result.Fail($"Cannot order. Purchase order remaining is 0.");
                        //}

                        //if (model.QtyReceived > purchaseOrder.Remaining)
                        //{
                        //    return Result.Fail($"Purchase order only have a remaining of: {purchaseOrder.Remaining}. Cannot order more than the purchase order remaining.");
                        //}

                        // get all receives by PurchaseOrderId
                        var receivesOrderQty = await _context.Receives
                            .Where(x => x.PurchaseOrderId == model.PurchaseOrderId)
                            .SumAsync(x => x.Qty);

                        var receiveRemaining = await _context.Receives
                            .Where(x => x.PurchaseOrderId == model.PurchaseOrderId)
                            .SumAsync(x => x.Remaining);

                        // update PurchaseOrder's OrderQty and Remaining
                        purchaseOrder.OrderQty = receivesOrderQty;
                        purchaseOrder.Remaining = receiveRemaining;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new ReceiveGetModel
                        {
                            ReceiveId = entity.ReceiveId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            PurchaseOrderId = entity.PurchaseOrderId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Received = entity.Received,
                            Remaining = entity.Remaining,
                            UpdatedDateTime = entity.UpdatedDateTime,
                            LotId = entity.LotId
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

        public async Task<Result<ReceiveGetModel>> UpdateReceiveQtyAsync(AppState state, ReceiveQtyUpdateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(ReceiveQtyUpdateModel)} is required.");
                }

                Receive receive;
                if (model.ReceiveId is null)
                {
                    return Result.Fail($"{nameof(ReceiveQtyUpdateModel.ReceiveId)} is required.");
                }
                else
                {
                    receive = await _context.Receives
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ReceiveId == model.ReceiveId);

                    if (receive is null)
                    {
                        return Result.Fail($"{nameof(Receive)} not found.");
                    }
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(ReceiveQtyUpdateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(ReceiveQtyUpdateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(ReceiveQtyUpdateModel.Qty)} cannot be zero.");
                    }
                }

                if (receive.Received > model.Qty.Value)
                {
                    return Result.Fail($"Cannot adjust the quantity below the received quantity: {receive.Received}.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.Receives
                            .SingleOrDefaultAsync(x => x.ReceiveId == model.ReceiveId);

                        entity.Remaining = model.Qty.Value - entity.Received;
                        entity.Qty = model.Qty.Value;
                        entity.UpdatedDateTime = DateTime.Now;
                        await _context.SaveChangesAsync();

                        // update po
                        var purchaseOrder = await _context.PurchaseOrders
                            .SingleOrDefaultAsync(x => x.PurchaseOrderId == entity.PurchaseOrderId);

                        // get all receives by PurchaseOrderId
                        var receivesOrderQty = await _context.Receives
                            .Where(x => x.PurchaseOrderId == entity.PurchaseOrderId)
                            .SumAsync(x => x.Qty);

                        var receiveRemaining = await _context.Receives
                            .Where(x => x.PurchaseOrderId == entity.PurchaseOrderId)
                            .SumAsync(x => x.Remaining);

                        // update PurchaseOrder's OrderQty and Remaining
                        purchaseOrder.OrderQty = receivesOrderQty;
                        purchaseOrder.Remaining = receiveRemaining;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new ReceiveGetModel
                        {
                            ReceiveId = entity.ReceiveId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            PurchaseOrderId = entity.PurchaseOrderId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Received = entity.Received,
                            Remaining = entity.Remaining,
                            UpdatedDateTime = entity.UpdatedDateTime,
                            LotId = entity.LotId
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

        public async Task<Result<ReceiveGetModel>> DeleteReceiveAsync(AppState state, ReceiveDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                Receive receive;
                if (model.ReceiveId is null)
                {
                    return Result.Fail($"{nameof(ReceiveDeleteModel.ReceiveId)} is required.");
                }
                else
                {
                    receive = await _context.Receives
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ReceiveId == model.ReceiveId);

                    if (receive is null)
                    {
                        return Result.Fail($"{nameof(Receive)} not found.");
                    }
                }

                if (receive.Received > 0)
                {
                    return Result.Fail($"Cannot cancel purchase order item because it started receiving.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.Receives
                            .SingleOrDefaultAsync(x => x.ReceiveId == model.ReceiveId);

                        entity.Deleted = true;
                        entity.UpdatedDateTime = DateTime.Now;
                        await _context.SaveChangesAsync();

                        // update po
                        var purchaseOrder = await _context.PurchaseOrders
                            .SingleOrDefaultAsync(x => x.PurchaseOrderId == entity.PurchaseOrderId);

                        // get all receives by PurchaseOrderId
                        var receivesOrderQty = await _context.Receives
                            .Where(x => x.PurchaseOrderId == entity.PurchaseOrderId)
                            .SumAsync(x => x.Qty);

                        var receiveRemaining = await _context.Receives
                            .Where(x => x.PurchaseOrderId == entity.PurchaseOrderId)
                            .SumAsync(x => x.Remaining);

                        // update PurchaseOrder's OrderQty and Remaining
                        purchaseOrder.OrderQty = receivesOrderQty;
                        purchaseOrder.Remaining = receiveRemaining;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new ReceiveGetModel
                        {
                            ReceiveId = entity.ReceiveId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            PurchaseOrderId = entity.PurchaseOrderId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Received = entity.Received,
                            Remaining = entity.Remaining,
                            UpdatedDateTime = entity.UpdatedDateTime,
                            LotId = entity.LotId
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

        public async Task<Result<IEnumerable<ReceiveLookupPOReceiveDeviceGetModel>>> GetReceiveLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int purchaseOrderId, string searchText, bool skuSearch = false)
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

                var query = _context.Receives
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.PurchaseOrder)
                    .Include(x => x.PutAways)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (skuSearch)
                    {
                        query = query.Where(x => x.PurchaseOrderId == purchaseOrderId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.PurchaseOrder.CustomerFacilityId == customerFacilityId
                            && x.Remaining != 0
                            && x.Item.SKU.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => x.PurchaseOrderId == purchaseOrderId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.PurchaseOrder.CustomerFacilityId == customerFacilityId
                            && x.Remaining != 0
                            && (x.Item.SKU.Trim().ToLower().Contains(searchText)
                                || x.Item.Description.Trim().ToLower().Contains(searchText)
                                || x.Remaining.ToString().Trim().ToLower().Contains(searchText)));
                    }
                }
                else
                {
                    query = query.Where(x => x.PurchaseOrderId == purchaseOrderId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.PurchaseOrder.CustomerFacilityId == customerFacilityId
                        && x.Remaining != 0);
                }

                IEnumerable<ReceiveLookupPOReceiveDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ReceiveLookupPOReceiveDeviceGetModel
                    {
                        ReceiveId = x.ReceiveId,
                        ItemId = x.Item.ItemId,
                        SKU = x.Item.SKU,
                        Description = x.Item.Description,
                        UOM = x.Item.UnitOfMeasure.Code,
                        Remaining = x.Remaining - x.PutAways.Sum(x => x.Remaining)
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail("Item not found.");
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