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
    public class ReceiptService : IReceiptService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public ReceiptService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<ReceiptGetModel>> CreateReceiptAsync(AppState state, ReceiptCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(ReceiptCreateModel.CustomerId)} is required.");
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
                    return Result.Fail($"{nameof(ReceiptCreateModel.CustomerLocationId)} is required.");
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

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(ReceiptCreateModel.ItemId)} is required.");
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

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(ReceiptCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(ReceiptCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(ReceiptCreateModel.Qty)} cannot be zero.");
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
                        var entity = new Receipt
                        {
                            CustomerLocationId = model.CustomerLocationId,
                            ItemId = model.ItemId,
                            Qty = model.Qty.Value,
                            Remaining = model.Qty.Value,
                            ReceiptDate = DateTime.Now,
                            LotId = model.LotId
                        };
                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var putAway = new PutAway
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            ReceiptId = entity.ReceiptId,
                            Qty = model.Qty.Value,
                            Remaining = model.Qty.Value,
                            PutAwayType = PutAwayTypeEnum.Receipt,
                            PutAwayDate = DateTime.Now
                        };
                        _context.Add(putAway);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new ReceiptGetModel
                        {
                            ReceiptId = entity.ReceiptId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Remaining = entity.Remaining,
                            ReceiptDate = entity.ReceiptDate,
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

        public async Task<Result<ReceiptQueueGetModel>> GetReceiptQueueAsync(AppState state, int customerLocationId, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var receipts = await _context.Receipts
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.PutAways)
                        .ThenInclude(x => x.PutAwayBins)
                            .ThenInclude(x => x.Bin)
                                .ThenInclude(x => x.Zone)
                    .Where(x => x.Remaining > 0
                        && x.CustomerLocationId == customerLocationId
                        && x.Item.Inventory.InventoryZones.Any(z => z.Zone.CustomerFacilityId == customerLocationId))
                    .OrderByDescending(x => x.ReceiptDate)
                    .ToListAsync();

                var model = new ReceiptQueueGetModel();
                var dto = new List<ReceiptQueueGetModel.Receipt>();
                foreach (var x in receipts)
                {
                    var r = new ReceiptQueueGetModel.Receipt();
                    r.ReceiptId = x.ReceiptId;
                    r.ItemId = x.Item.ItemId;
                    r.ItemSKU = x.Item.SKU;
                    r.ItemDescription = x.Item.Description;
                    r.ItemUOM = x.Item.UnitOfMeasure.Code;
                    r.ReceiptDate = x.ReceiptDate;
                    r.Qty = x.Qty;
                    r.Remaining = x.Remaining;

                    var locations = new List<ReceiptQueueGetModel.Location>();
                    foreach (var z in x.PutAways)
                    {
                        foreach (var pb in z.PutAwayBins.OrderByDescending(zz => zz.ReceivedDateTime))
                        {
                            var l = new ReceiptQueueGetModel.Location();
                            l.ZoneId = pb.Bin.Zone.ZoneId;
                            l.Zone = pb.Bin.Zone.Name;
                            l.BinId = pb.Bin.BinId;
                            l.Bin = pb.Bin.Name;
                            l.Qty = pb.Qty;
                            l.ReceivedDateTime = pb.ReceivedDateTime;

                            locations.Add(l);
                        }
                    }

                    r.Locations = locations;

                    dto.Add(r);
                }
                model.Receipts = dto;

                if (model is null)
                {
                    return Result.Fail($"{nameof(Receipt)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ReceiptGetModel>> CreateReceiptDeviceAsync(CustomerDeviceTokenAuthModel state, ReceiptDeviceCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(ReceiptDeviceCreateModel.ItemId)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.ItemId == model.ItemId
                            && x.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail("Item not found.");
                    }
                }

                if (model.Qty is null)
                {
                    return Result.Fail($"{nameof(ReceiptDeviceCreateModel.Qty)} is required.");
                }
                else
                {
                    if (model.Qty.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(ReceiptDeviceCreateModel.Qty)} cannot be negative.");
                    }
                    else if (model.Qty.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(ReceiptDeviceCreateModel.Qty)} cannot be zero.");
                    }
                }

                if (model.NewLotGetCreate is not null)
                {
                    if (model.NewLotGetCreate.LotId is null) // create
                    {
                        if (!model.NewLotGetCreate.LotNo.HasValue())
                        {
                            return Result.Fail($"{nameof(LotCreateModel.LotNo)} is required.");
                        }
                        else
                        {
                            var exist = await _context.Lots
                                .AnyAsync(x => x.LotNo.Trim().ToLower() == model.NewLotGetCreate.LotNo.Trim().ToLower()
                                    && x.ItemId == model.ItemId);

                            if (exist)
                            {
                                return Result.Fail($"{nameof(LotCreateModel.LotNo)} is already exist.");
                            }
                        }

                        if (model.NewLotGetCreate.ExpirationDate is null)
                        {
                            return Result.Fail($"{nameof(LotCreateModel.ExpirationDate)} is required.");
                        }
                    }
                    else
                    {
                        var exist = await _context.Lots
                            .AnyAsync(x => x.LotId == model.NewLotGetCreate.LotId
                                && x.ItemId == model.ItemId);

                        if (!exist)
                        {
                            return Result.Fail("Lot not found.");
                        }
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = new Receipt
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            ItemId = model.ItemId,
                            Qty = model.Qty.Value,
                            Remaining = model.Qty.Value,
                            ReceiptDate = DateTime.Now,
                        };

                        if (model.NewLotGetCreate is not null)
                        {
                            if (model.NewLotGetCreate.LotId is null) // create
                            {
                                var lot = new Lot
                                {
                                    CustomerLocationId = state.CustomerLocationId,
                                    ItemId = model.ItemId,
                                    LotNo = model.NewLotGetCreate.LotNo,
                                    ExpirationDate = model.NewLotGetCreate.ExpirationDate.Value
                                };
                                _context.Add(lot);
                                await _context.SaveChangesAsync();

                                entity.LotId = lot.LotId;
                            }
                            else
                            {
                                entity.LotId = model.NewLotGetCreate.LotId;
                            }
                        }

                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var putAway = new PutAway
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            ReceiptId = entity.ReceiptId,
                            Qty = model.Qty.Value,
                            Remaining = model.Qty.Value,
                            PutAwayType = PutAwayTypeEnum.Receipt,
                            PutAwayDate = DateTime.Now
                        };
                        _context.Add(putAway);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new ReceiptGetModel
                        {
                            ReceiptId = entity.ReceiptId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            ItemId = entity.ItemId.Value,
                            Qty = entity.Qty,
                            Remaining = entity.Remaining,
                            ReceiptDate = entity.ReceiptDate,
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
    }
}