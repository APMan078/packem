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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class TransferService : ITransferService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public TransferService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<TransferGetModel>> CreateTransferAsync(AppState state, TransferCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(TransferCreateModel.CustomerId)} is required.");
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
                    return Result.Fail($"{nameof(TransferCreateModel.CustomerLocationId)} is required.");
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
                    return Result.Fail($"{nameof(TransferCreateModel.ItemId)} is required.");
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

                if (model.ItemFacilityId is null)
                {
                    return Result.Fail($"{nameof(TransferCreateModel.ItemFacilityId)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerFacilities
                        .AnyAsync(x => x.CustomerFacilityId == model.ItemFacilityId
                            && x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == model.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail("Facility not found.");
                    }
                }

                Bin currentBin;
                if (model.ItemBinId is null)
                {
                    return Result.Fail($"{nameof(TransferCreateModel.ItemBinId)} is required.");
                }
                else
                {
                    currentBin = await _context.Bins
                        .Include(x => x.Zone)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.BinId == model.ItemBinId
                            && x.CustomerLocationId == model.CustomerLocationId
                            && x.Zone.CustomerFacilityId == model.ItemFacilityId);

                    if (currentBin is null)
                    {
                        return Result.Fail("Bin not found.");
                    }
                }

                if (model.NewZoneId is null && model.NewBinId is not null)
                {
                    return Result.Fail($"{nameof(TransferCreateModel.NewZoneId)} is required.");
                }

                if (model.NewZoneId is not null && model.NewBinId is not null)
                {
                    var exist = await _context.Zones
                        .AnyAsync(x => x.ZoneId == model.NewZoneId
                            && x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerFacilityId == model.ItemFacilityId);

                    if (!exist)
                    {
                        return Result.Fail("Zone not found.");
                    }

                    var exist2 = await _context.Bins
                        .AnyAsync(x => x.BinId == model.NewBinId
                            && x.CustomerLocationId == model.CustomerLocationId
                            && x.ZoneId == model.NewZoneId);

                    if (!exist2)
                    {
                        return Result.Fail("Bin not found.");
                    }
                    else
                    {
                        if (model.ItemBinId == model.NewBinId)
                        {
                            return Result.Fail($"Cannot transfer with the same bin location.");
                        }
                    }
                }

                if (model.QtyToTransfer is null)
                {
                    return Result.Fail($"{nameof(TransferCreateModel.QtyToTransfer)} is required.");
                }
                else
                {
                    if (model.QtyToTransfer.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(TransferCreateModel.QtyToTransfer)} cannot be negative.");
                    }
                    else if (model.QtyToTransfer.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(TransferCreateModel.QtyToTransfer)} cannot be zero.");
                    }
                }

                var bin = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BinId == model.ItemBinId
                        && x.Inventory.ItemId == model.ItemId
                        && x.CustomerLocationId == model.CustomerLocationId);

                if (bin == null)
                {
                    return Result.Fail($"Item doesn't exist in this bin location");
                }
                else
                {
                    var transferQty = await _context.Transfers
                        .Include(x => x.TransferCurrent)
                        .AsNoTracking()
                        .Where(x => x.ItemId == model.ItemId
                            && (x.Status == TransferStatusEnum.Pending || x.Status == TransferStatusEnum.InProgress)
                            && x.TransferCurrent.CurrentBinId == model.ItemBinId
                            && x.CustomerLocationId == model.CustomerLocationId)
                        .SumAsync(x => x.Remaining);

                    if (transferQty == 0)
                    {
                        if (model.QtyToTransfer > bin.Qty)
                        {
                            return Result.Fail($"Cannot transfer more than the quantity of the item in the selected bin location which only have: {bin.Qty}.");
                        }
                    }
                    else
                    {
                        var qty = bin.Qty - transferQty;
                        if (model.QtyToTransfer > qty)
                        {
                            return Result.Fail($"Cannot transfer. Item quantity at bin location: {bin.Qty}. And you have a pending item to transfer with a quantity of: {transferQty}. The quantity you can transfer: {qty}.");
                        }
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var transferCurrent = new TransferCurrent
                        {
                            CustomerLocationId = model.CustomerLocationId,
                            CurrentZoneId = currentBin.ZoneId,
                            CurrentBinId = model.ItemBinId
                        };
                        _context.Add(transferCurrent);
                        await _context.SaveChangesAsync();

                        TransferNew transferNew = null;
                        if (model.NewZoneId is not null)
                        {
                            transferNew = new TransferNew
                            {
                                CustomerLocationId = model.CustomerLocationId,
                                NewZoneId = model.NewZoneId,
                                NewBinId = model.NewBinId
                            };
                            _context.Add(transferNew);
                            await _context.SaveChangesAsync();

                            if (model.NewBinId is not null)
                            {
                                var binFrom = await _context.InventoryBins
                                    .Include(x => x.Lot)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.BinId == model.ItemBinId
                                        && x.Inventory.ItemId == model.ItemId
                                        && x.CustomerLocationId == model.CustomerLocationId);

                                var binTo = await _context.InventoryBins
                                    .Include(x => x.Lot)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.BinId == model.NewBinId
                                        && x.Inventory.ItemId == model.ItemId
                                        && x.CustomerLocationId == model.CustomerLocationId);

                                if (binFrom?.LotId != binTo?.LotId)
                                {
                                    await transaction.RollbackAsync();

                                    if (binFrom?.LotId is not null
                                        || binTo?.LotId is not null)
                                    {
                                        var lotFrom = binFrom?.LotId is not null ? binFrom?.Lot.LotNo : "NO LOT NUMBER";
                                        var lotTo = binTo?.LotId is not null ? binTo?.Lot.LotNo : "NO LOT NUMBER";

                                        return Result.Fail($"Cannot transfer item with different lot number in the same bin location. Item's lot number: FROM: '{lotFrom}', TO: '{lotTo}'.");
                                    }
                                }
                            }
                        }

                        var entity = new Transfer
                        {
                            CustomerLocationId = model.CustomerLocationId,
                            ItemId = model.ItemId,
                            TransferCurrentId = transferCurrent.TransferCurrentId,
                            TransferNewId = transferNew is not null ? transferNew.TransferNewId : null,
                            Qty = model.QtyToTransfer.Value,
                            Remaining = model.QtyToTransfer.Value,
                            Status = TransferStatusEnum.Pending,
                            TransferDateTime = DateTime.Now
                        };
                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new TransferGetModel
                        {
                            TransferId = entity.TransferId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            ItemId = entity.ItemId.Value,
                            TransferCurrentId = entity.TransferCurrentId.Value,
                            TransferNewId = entity.TransferNewId,
                            Qty = entity.Qty,
                            Remaining = entity.Remaining,
                            Status = entity.Status,
                            TransferDateTime = entity.TransferDateTime,
                            ReceivedDateTime = entity.CompletedDateTime
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

        public async Task<Result<TransferHistoryGetModel>> GetTransferHistoryAsync(AppState state, int customerLocationId, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var transfer = await _context.Transfers
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.TransferCurrent)
                        .ThenInclude(x => x.CurrentZone)
                    .Include(x => x.TransferCurrent)
                        .ThenInclude(x => x.CurrentBin)
                    .Include(x => x.TransferNew)
                        .ThenInclude(x => x.NewZone)
                    .Include(x => x.TransferNew)
                        .ThenInclude(x => x.NewBin)
                    .Include(x => x.TransferZoneBins)
                        .ThenInclude(x => x.Zone)
                    .Include(x => x.TransferZoneBins)
                        .ThenInclude(x => x.Bin)
                    .AsNoTracking()
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.TransferCurrent.CurrentZone.CustomerFacilityId == customerFacilityId)
                    .ToListAsync();

                var model = new TransferHistoryGetModel();
                model.Pending = transfer.Where(x => x.Status == TransferStatusEnum.Pending).Count();
                model.Completed = transfer.Where(x => x.Status == TransferStatusEnum.Completed).Count();
                model.UnitToday = transfer.Select(x => x.TransferZoneBins.Where(x => x.ReceivedDateTime == DateTime.Now).Sum(x => x.Qty)).Sum(x => x);

                var dto = new List<TransferHistoryGetModel.Transfer>();
                foreach (var x in transfer.OrderByDescending(x => x.TransferDateTime))
                {
                    var t = new TransferHistoryGetModel.Transfer();
                    t.TransferId = x.TransferId;
                    t.Status = x.Status.ToLabel();
                    t.ItemId = x.Item.ItemId;
                    t.ItemSKU = x.Item.SKU;
                    t.ItemDescription = x.Item.Description;
                    t.ItemUOM = x.Item.UnitOfMeasure.Code;
                    t.FromZoneId = x.TransferCurrent.CurrentZone.ZoneId;
                    t.FromZone = x.TransferCurrent.CurrentZone.Name;
                    t.FromBinId = x.TransferCurrent.CurrentBin.BinId;
                    t.FromBin = x.TransferCurrent.CurrentBin.Name;
                    t.QtyTransfer = x.Qty;

                    if (x.TransferNewId is null && x.TransferZoneBins.Count == 0)
                    {
                        t.ToZone = "Any";
                        t.ToBin = "Any";
                    }
                    else if (x.TransferNewId is not null && x.TransferZoneBins.Count == 0)
                    {
                        if (x.TransferNew.NewZone is null)
                        {
                            t.ToZone = "Any";
                        }
                        else
                        {
                            t.ToZoneId = x.TransferNew.NewZone.ZoneId;
                            t.ToZone = x.TransferNew.NewZone.Name;
                        }

                        if (x.TransferNew.NewBin is null)
                        {
                            t.ToBin = "Any";
                        }
                        else
                        {
                            t.ToBinId = x.TransferNew.NewBin.BinId;
                            t.ToBin = x.TransferNew.NewBin.Name;
                        }
                    }
                    else
                    {
                        t.ToTransferLocations = x.TransferZoneBins.OrderByDescending(x => x.ReceivedDateTime)
                            .Select(z => new TransferHistoryGetModel.ToTransferLocation
                            {
                                ToZoneId = z.Zone.ZoneId,
                                ToZone = z.Zone.Name,
                                ToBinId = z.Bin.BinId,
                                ToBin = z.Bin.Name
                            });
                    }

                    dto.Add(t);
                }
                model.Transfers = dto;

                if (model is null)
                {
                    return Result.Fail($"{nameof(Transfer)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<TransferLookupDeviceGetModel>>> GetTransferLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false)
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

                var query = _context.Transfers
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.InventoryZones)
                                .ThenInclude(x => x.Zone)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.InventoryBins)
                                .ThenInclude(x => x.Inventory)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.InventoryBins)
                                .ThenInclude(x => x.Lot)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.UnitOfMeasure)
                    .Include(x => x.TransferCurrent)
                        .ThenInclude(x => x.CurrentZone)
                    .Include(x => x.TransferCurrent)
                        .ThenInclude(x => x.CurrentBin)
                    .Include(x => x.TransferNew)
                        .ThenInclude(x => x.NewZone)
                    .Include(x => x.TransferNew)
                        .ThenInclude(x => x.NewBin)
                    .Include(x => x.CustomerLocation)
                        .ThenInclude(x => x.CustomerFacilities)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (skuSearch)
                    {
                        query = query.Where(x => (x.Status == TransferStatusEnum.Pending || x.Status == TransferStatusEnum.InProgress)
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.Item.Inventory.InventoryZones.Any(z => z.Zone.CustomerFacilityId == customerFacilityId)
                            && x.Item.SKU.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => (x.Status == TransferStatusEnum.Pending || x.Status == TransferStatusEnum.InProgress)
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.Item.Inventory.InventoryZones.Any(z => z.Zone.CustomerFacilityId == customerFacilityId)
                            && (x.Item.SKU.Trim().ToLower().Contains(searchText)
                                || x.Item.Description.Trim().ToLower().Contains(searchText)));
                    }
                }
                else
                {
                    query = query.Where(x => (x.Status == TransferStatusEnum.Pending || x.Status == TransferStatusEnum.InProgress)
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.Item.Inventory.InventoryZones.Any(z => z.Zone.CustomerFacilityId == customerFacilityId));
                }

                IEnumerable<TransferLookupDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new TransferLookupDeviceGetModel
                    {
                        TransferId = x.TransferId,
                        ItemId = x.Item.ItemId,
                        ItemSKU = x.Item.SKU,
                        ItemDescription = x.Item.Description,
                        ItemUOM = x.Item.UnitOfMeasure.Code,
                        CurrentZone = x.TransferCurrent.CurrentZone.Name,
                        CurrentZoneId = x.TransferCurrent.CurrentZone.ZoneId,
                        CurrentBin = x.TransferCurrent.CurrentBin.Name,
                        CurrentBinId = x.TransferCurrent.CurrentBin.BinId,
                        CurrentBinQty = x.Item.Inventory.InventoryBins
                            .SingleOrDefault(z => z.Inventory.ItemId == x.ItemId && z.BinId == x.TransferCurrent.CurrentBinId) == null ? 0
                            : x.Item.Inventory.InventoryBins
                                .SingleOrDefault(z => z.Inventory.ItemId == x.ItemId && z.BinId == x.TransferCurrent.CurrentBinId).Qty,
                        NewZone = x.TransferNew.NewZone.Name,
                        NewZoneId = x.TransferNew.NewZoneId,
                        NewBin = x.TransferNew.NewBin.Name,
                        NewBinId = x.TransferNew.NewBinId,
                        QtyToTransfer = x.Qty,
                        Remaining = x.Remaining,
                        LotNo = x.Item.Inventory.InventoryBins
                            .SingleOrDefault(z => z.Inventory.ItemId == x.ItemId && z.BinId == x.TransferCurrent.CurrentBinId) == null ? null
                            : x.Item.Inventory.InventoryBins
                                .SingleOrDefault(z => z.Inventory.ItemId == x.ItemId && z.BinId == x.TransferCurrent.CurrentBinId).Lot.LotNo,
                        ExpirationDate = x.Item.Inventory.InventoryBins
                            .SingleOrDefault(z => z.Inventory.ItemId == x.ItemId && z.BinId == x.TransferCurrent.CurrentBinId) == null ? null
                            : x.Item.Inventory.InventoryBins
                                .SingleOrDefault(z => z.Inventory.ItemId == x.ItemId && z.BinId == x.TransferCurrent.CurrentBinId).Lot.ExpirationDate
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

        public async Task<Result<TransferGetModel>> CreateTransferManualDeviceAsync(CustomerDeviceTokenAuthModel state, TransferManualCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
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

                if (model.ItemId is null)
                {
                    return Result.Fail($"{nameof(TransferManualCreateModel.ItemId)} is required.");
                }
                else
                {
                    var exist = await _context.Items
                        .AnyAsync(x => x.ItemId == model.ItemId
                            && x.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Item)} not found.");
                    }
                }

                if (model.ItemFacilityId is null)
                {
                    return Result.Fail($"{nameof(TransferManualCreateModel.ItemFacilityId)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerFacilities
                        .AnyAsync(x => x.CustomerFacilityId == model.ItemFacilityId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail("Facility not found.");
                    }
                }

                Zone currentZone;
                if (model.ItemZoneId is null)
                {
                    return Result.Fail($"{nameof(TransferManualCreateModel.ItemZoneId)} is required.");
                }
                else
                {
                    currentZone = await _context.Zones
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ZoneId == model.ItemZoneId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == model.ItemFacilityId);

                    if (currentZone is null)
                    {
                        return Result.Fail("Area not found.");
                    }
                }

                Bin currentBin;
                if (model.ItemBinId is null)
                {
                    return Result.Fail($"{nameof(TransferManualCreateModel.ItemBinId)} is required.");
                }
                else
                {
                    currentBin = await _context.Bins
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.BinId == model.ItemBinId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.ZoneId == model.ItemZoneId);

                    if (currentBin is null)
                    {
                        return Result.Fail("Location not found.");
                    }
                }

                if (model.NewBinGetCreate is null)
                {
                    return Result.Fail($"{nameof(model.NewBinGetCreate)} is required.");
                }

                if (model.NewBinGetCreate.ZoneId is null)
                {
                    return Result.Fail($"{nameof(model.NewBinGetCreate)}.{nameof(model.NewBinGetCreate.ZoneId)} is required.");
                }
                else
                {
                    var exist = await _context.Zones
                        .AnyAsync(x => x.ZoneId == model.NewBinGetCreate.ZoneId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.CustomerFacilityId == model.ItemFacilityId);

                    if (!exist)
                    {
                        return Result.Fail("Area not found.");
                    }
                }

                if (!model.NewBinGetCreate.Name.HasValue())
                {
                    return Result.Fail($"{nameof(model.NewBinGetCreate)}.{nameof(model.NewBinGetCreate.Name)} is required.");
                }

                if (model.NewBinGetCreate is not null)
                {
                    var newBin = await _context.Bins
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ZoneId == model.NewBinGetCreate.ZoneId
                                && x.Name.Trim() == model.NewBinGetCreate.Name.Trim()
                                && x.CustomerLocationId == state.CustomerLocationId);

                    if (newBin is not null)
                    {
                        if (model.ItemBinId == newBin.BinId)
                        {
                            return Result.Fail($"Cannot transfer with the same location.");
                        }
                    }
                }

                if (model.QtyToTransfer is null)
                {
                    return Result.Fail($"{nameof(TransferManualCreateModel.QtyToTransfer)} is required.");
                }
                else
                {
                    if (model.QtyToTransfer.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(TransferManualCreateModel.QtyToTransfer)} cannot be negative.");
                    }
                    else if (model.QtyToTransfer.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(TransferManualCreateModel.QtyToTransfer)} cannot be zero.");
                    }
                }

                var bin = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Lot)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BinId == model.ItemBinId
                        && x.Inventory.ItemId == model.ItemId
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (bin == null)
                {
                    return Result.Fail($"Item doesn't exist in this location");
                }
                else
                {
                    var transferQty = await _context.Transfers
                        .Include(x => x.TransferCurrent)
                        .AsNoTracking()
                        .Where(x => x.ItemId == model.ItemId
                            && (x.Status == TransferStatusEnum.Pending || x.Status == TransferStatusEnum.InProgress)
                            && x.TransferCurrent.CurrentBinId == model.ItemBinId
                            && x.CustomerLocationId == state.CustomerLocationId)
                        .SumAsync(x => x.Remaining);

                    if (transferQty == 0)
                    {
                        if (model.QtyToTransfer > bin.Qty)
                        {
                            return Result.Fail($"Cannot transfer more than the quantity of the item in the selected location which only have: {bin.Qty}.");
                        }
                    }
                    else
                    {
                        var qty = bin.Qty - transferQty;
                        if (model.QtyToTransfer > qty)
                        {
                            return Result.Fail($"Cannot transfer. Item quantity at location: {bin.Qty}. And you have a pending item to transfer with a quantity of: {transferQty}. The quantity you can transfer: {qty}.");
                        }
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Bin binDb = await _context.Bins
                            .SingleOrDefaultAsync(x => x.ZoneId == model.NewBinGetCreate.ZoneId
                                && x.Name.Trim() == model.NewBinGetCreate.Name.Trim()
                                && x.CustomerLocationId == state.CustomerLocationId);
                        if (binDb is null)
                        {
                            binDb = new Bin
                            {
                                CustomerLocationId = state.CustomerLocationId,
                                ZoneId = model.NewBinGetCreate.ZoneId,
                                Name = model.NewBinGetCreate.Name
                            };
                            _context.Add(binDb);
                            await _context.SaveChangesAsync();
                        }

                        var transferCurrent = new TransferCurrent
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            CurrentZoneId = model.ItemZoneId,
                            CurrentBinId = model.ItemBinId
                        };
                        _context.Add(transferCurrent);
                        await _context.SaveChangesAsync();

                        var transferNew = new TransferNew
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            NewZoneId = binDb.ZoneId,
                            NewBinId = binDb.BinId
                        };
                        _context.Add(transferNew);
                        await _context.SaveChangesAsync();

                        var entity = new Transfer
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            ItemId = model.ItemId,
                            TransferCurrentId = transferCurrent.TransferCurrentId,
                            TransferNewId = transferNew.TransferNewId,
                            Qty = model.QtyToTransfer.Value,
                            Remaining = 0,
                            Status = TransferStatusEnum.Manual,
                            TransferDateTime = DateTime.Now,
                            CompletedDateTime = DateTime.Now
                        };
                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        var transfer = await _context.Transfers
                            .Include(x => x.TransferCurrent)
                            .Include(x => x.TransferNew)
                            .Include(x => x.Item)
                                .ThenInclude(x => x.Inventory)
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.TransferId == entity.TransferId);

                        var transferZoneBin = new TransferZoneBin
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            TransferId = entity.TransferId,
                            ZoneId = transferNew.NewZoneId,
                            BinId = transferNew.NewBinId,
                            Qty = model.QtyToTransfer.Value,
                            ReceivedDateTime = DateTime.Now
                        };
                        _context.Add(transferZoneBin);
                        await _context.SaveChangesAsync();

                        // sum
                        var currentInventoryBin = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .SingleOrDefaultAsync(x => x.BinId == transfer.TransferCurrent.CurrentBinId
                                && x.Inventory.ItemId == transfer.ItemId
                                && x.CustomerLocationId == transfer.CustomerLocationId);

                        var currentInventoryBinQty = currentInventoryBin.Qty;

                        var newInventoryBin = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Lot)
                            .SingleOrDefaultAsync(x => x.BinId == transfer.TransferNew.NewBinId
                                && x.Inventory.ItemId == transfer.ItemId
                                && x.CustomerLocationId == transfer.CustomerLocationId);

                        if (newInventoryBin is null)
                        {
                            newInventoryBin = new InventoryBin
                            {
                                CustomerLocationId = transfer.CustomerLocationId,
                                InventoryId = transfer.Item.Inventory.InventoryId,
                                BinId = transfer.TransferNew.NewBinId,
                                Qty = 0,
                                LotId = bin.LotId
                            };
                            _context.Add(newInventoryBin);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            if (newInventoryBin.LotId != bin.LotId)
                            {
                                await transaction.RollbackAsync();

                                if (newInventoryBin.LotId is not null
                                    || bin.LotId is not null)
                                {
                                    var lotFrom = bin.LotId is not null ? bin.Lot.LotNo : "NO LOT NUMBER";
                                    var lotTo = newInventoryBin.LotId is not null ? newInventoryBin.Lot.LotNo : "NO LOT NUMBER";

                                    return Result.Fail($"Cannot transfer item with different lot number in the same location. Item's lot number: FROM: '{lotFrom}', TO: '{lotTo}'.");
                                }
                            }
                        }

                        var newInventoryBinQty = newInventoryBin.Qty;

                        // update inventory qty, zone qty and bin qty
                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == transfer.ItemId);

                        var oldQty = inventory.QtyOnHand;

                        // decrease qty of current bin and zone
                        currentInventoryBin.Qty = currentInventoryBinQty - transfer.Qty;
                        await _context.SaveChangesAsync();

                        // delete current if zero
                        if (currentInventoryBin.Qty.IsZero())
                        {
                            currentInventoryBin.Deleted = true;
                            await _context.SaveChangesAsync();
                        }

                        // sum all bins of selected zoneId
                        var currentInventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.ZoneId == transfer.TransferCurrent.CurrentZoneId);

                        var sumCurrentInventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == transfer.ItemId
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

                        // increase qty of new bin  and zone
                        newInventoryBin.Qty = newInventoryBinQty + transfer.Qty;
                        await _context.SaveChangesAsync();

                        // sum all bins of selected zoneId
                        var newInventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.ZoneId == transfer.TransferNew.NewZoneId);

                        if (newInventoryZone is null)
                        {
                            newInventoryZone = new InventoryZone
                            {
                                CustomerLocationId = transfer.CustomerLocationId,
                                InventoryId = inventory.InventoryId,
                                ZoneId = transfer.TransferNew.NewZoneId,
                                Qty = 0
                            };
                            _context.Add(newInventoryZone);
                            await _context.SaveChangesAsync();
                        }

                        var sumNewInventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == transfer.ItemId
                                && x.Bin.ZoneId == newInventoryZone.ZoneId)
                            .SumAsync(x => x.Qty);

                        newInventoryZone.Qty = sumNewInventoryBinQty;
                        await _context.SaveChangesAsync();

                        // sum all zones of selected inventory
                        var inventoryZoneQty = await _context.InventoryZones
                            .Include(x => x.Inventory)
                            .Where(x => x.Inventory.ItemId == transfer.ItemId)
                            .SumAsync(x => x.Qty);

                        // sum selected inventory in pallet
                        var inventoryPalletQty = await _context.PalletInventories
                            .Include(x => x.Inventory)
                            .Where(x => x.Inventory.ItemId == transfer.ItemId)
                            .SumAsync(x => x.Qty);

                        var inventoryQty = inventoryZoneQty + inventoryPalletQty;

                        inventory.QtyOnHand = inventoryQty;

                        await _context.SaveChangesAsync();

                        // create log
                        var activityLog = new ActivityLog
                        {
                            CustomerId = state.CustomerId,
                            Type = ActivityLogTypeEnum.TransferManual,
                            InventoryId = inventory.InventoryId,
                            UserId = model.UserId,
                            ActivityDateTime = DateTime.Now,
                            Qty = model.QtyToTransfer.Value,
                            OldQty = oldQty,
                            NewQty = inventoryQty,
                            MathematicalSymbol = null,
                            ZoneId = transferNew.NewZoneId,
                            BinId = transferNew.NewBinId
                        };
                        _context.Add(activityLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new TransferGetModel
                        {
                            TransferId = entity.TransferId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            ItemId = entity.ItemId.Value,
                            TransferCurrentId = entity.TransferCurrentId.Value,
                            TransferNewId = entity.TransferNewId,
                            Qty = entity.Qty,
                            Remaining = entity.Remaining,
                            Status = entity.Status,
                            TransferDateTime = entity.TransferDateTime,
                            ReceivedDateTime = entity.CompletedDateTime
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

        public async Task<Result<TransferGetModel>> CreateTransferRequestDeviceAsync(CustomerDeviceTokenAuthModel state, TransferRequestCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
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
                    return Result.Fail($"{nameof(TransferRequestCreateModel.UserId)} is required.");
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

                var transfer = await _context.Transfers
                    .AsNoTracking()
                    .Include(x => x.TransferCurrent)
                    .Include(x => x.TransferNew)
                        .ThenInclude(x => x.NewBin)
                    .SingleOrDefaultAsync(x => x.TransferId == model.TransferId
                        && (x.Status == TransferStatusEnum.Pending || x.Status == TransferStatusEnum.InProgress));

                if (transfer is null)
                {
                    return Result.Fail($"{nameof(Transfer)} not found.");
                }

                var item = await _context.Items
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ItemId == transfer.ItemId
                        && x.CustomerId == state.CustomerId);

                if (item is null)
                {
                    return Result.Fail("Item not found.");
                }

                if (transfer.TransferNew == null) // new zone and new bin required
                {
                    if (model.NewBinGetCreate is null)
                    {
                        return Result.Fail($"{nameof(model.NewBinGetCreate)} is required.");
                    }

                    if (model.NewBinGetCreate.ZoneId is null)
                    {
                        return Result.Fail($"{nameof(model.NewBinGetCreate)}.{nameof(model.NewBinGetCreate.ZoneId)} is required.");
                    }
                    else
                    {
                        var exist = await _context.Zones
                            .AnyAsync(x => x.ZoneId == model.NewBinGetCreate.ZoneId
                                && x.CustomerLocationId == state.CustomerLocationId);

                        if (!exist)
                        {
                            return Result.Fail("Area not found.");

                        }
                    }

                    if (!model.NewBinGetCreate.Name.HasValue())
                    {
                        return Result.Fail($"{nameof(model.NewBinGetCreate)}.{nameof(model.NewBinGetCreate.Name)} is required.");
                    }
                    else
                    {
                        var newBin = await _context.Bins
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.ZoneId == model.NewBinGetCreate.ZoneId
                                && x.Name.Trim() == model.NewBinGetCreate.Name.Trim()
                                && x.CustomerLocationId == state.CustomerLocationId);

                        if (newBin is not null)
                        {
                            if (transfer.TransferCurrent.CurrentBinId == newBin.BinId)
                            {
                                return Result.Fail($"Cannot transfer with the same location.");
                            }
                        }
                    }
                }
                else if (transfer.TransferNew is not null && transfer.TransferNew.NewBinId == null) // new bin is required
                {
                    if (model.NewBinGetCreate is null)
                    {
                        return Result.Fail($"{nameof(model.NewBinGetCreate)} is required.");
                    }

                    if (!model.NewBinGetCreate.Name.HasValue())
                    {
                        return Result.Fail($"{nameof(model.NewBinGetCreate)}.{nameof(model.NewBinGetCreate.Name)} is required.");
                    }
                    else
                    {
                        var newBin = await _context.Bins
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.ZoneId == model.NewBinGetCreate.ZoneId
                                && x.Name.Trim() == model.NewBinGetCreate.Name.Trim()
                                && x.CustomerLocationId == state.CustomerLocationId);

                        if (newBin is not null)
                        {
                            if (transfer.TransferCurrent.CurrentBinId == newBin.BinId)
                            {
                                return Result.Fail($"Cannot transfer with the same location.");
                            }
                        }
                    }

                    model.NewBinGetCreate.ZoneId = transfer.TransferNew.NewZoneId;
                }
                else
                {
                    model.NewBinGetCreate = new BinGetCreateModel()
                    {
                        ZoneId = transfer.TransferNew.NewZoneId,
                        Name = transfer.TransferNew.NewBin.Name
                    };
                }

                if (model.QtyTransfered is null)
                {
                    return Result.Fail($"{nameof(TransferRequestCreateModel.QtyTransfered)} is required.");
                }
                else
                {
                    if (model.QtyTransfered.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(TransferRequestCreateModel.QtyTransfered)} cannot be negative.");
                    }
                    else if (model.QtyTransfered.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(TransferRequestCreateModel.QtyTransfered)} cannot be zero.");
                    }
                }

                if (model.QtyTransfered > transfer.Remaining)
                {
                    return Result.Fail($"Cannot transfer more than the requested quantity of: {transfer.Remaining}.");
                }

                var bin = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Lot)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BinId == transfer.TransferCurrent.CurrentBinId
                        && x.Inventory.ItemId == transfer.ItemId
                        && x.CustomerLocationId == state.CustomerLocationId);

                if (bin == null)
                {
                    return Result.Fail($"Item doesn't exist in this location.");
                }
                else
                {
                    if (model.QtyTransfered > bin.Qty)
                    {
                        return Result.Fail($"Cannot transfer more than the quantity of the item in the selected location which only have: {bin.Qty}.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Bin binDb = await _context.Bins
                            .SingleOrDefaultAsync(x => x.ZoneId == model.NewBinGetCreate.ZoneId
                                && x.Name.Trim() == model.NewBinGetCreate.Name.Trim()
                                && x.CustomerLocationId == state.CustomerLocationId);
                        if (binDb is null)
                        {
                            binDb = new Bin
                            {
                                CustomerLocationId = state.CustomerLocationId,
                                ZoneId = model.NewBinGetCreate.ZoneId,
                                Name = model.NewBinGetCreate.Name
                            };
                            _context.Add(binDb);
                            await _context.SaveChangesAsync();
                        }

                        var transferZoneBin = new TransferZoneBin
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            TransferId = model.TransferId,
                            ZoneId = model.NewBinGetCreate.ZoneId,
                            BinId = binDb.BinId,
                            Qty = model.QtyTransfered.Value,
                            ReceivedDateTime = DateTime.Now
                        };
                        _context.Add(transferZoneBin);
                        await _context.SaveChangesAsync();

                        // update transfer remaining
                        var transferDb = await _context.Transfers
                            .Include(x => x.TransferCurrent)
                            .Include(x => x.Item)
                                .ThenInclude(x => x.Inventory)
                            .SingleOrDefaultAsync(x => x.TransferId == model.TransferId);

                        var transferDbQty = transferDb.Qty;

                        var transferZoneBinQty = await _context.TransferZoneBins
                            .Where(x => x.TransferId == model.TransferId)
                            .SumAsync(x => x.Qty);
                        transferDb.Remaining = transferDbQty - transferZoneBinQty;
                        transferDb.Status = TransferStatusEnum.InProgress;
                        await _context.SaveChangesAsync();

                        if (transferDb.Remaining == 0)
                        {
                            transferDb.Status = TransferStatusEnum.Completed;
                            transferDb.CompletedDateTime = DateTime.Now;
                            await _context.SaveChangesAsync();
                        }

                        // sum
                        var currentInventoryBin = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .SingleOrDefaultAsync(x => x.BinId == transferDb.TransferCurrent.CurrentBinId
                                && x.Inventory.ItemId == transferDb.ItemId
                                && x.CustomerLocationId == transferDb.CustomerLocationId);

                        var currentInventoryBinQty = currentInventoryBin.Qty;

                        var newInventoryBin = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Lot)
                            .SingleOrDefaultAsync(x => x.BinId == binDb.BinId
                                && x.Inventory.ItemId == transferDb.ItemId
                                && x.CustomerLocationId == transferDb.CustomerLocationId);

                        if (newInventoryBin is null)
                        {
                            newInventoryBin = new InventoryBin
                            {
                                CustomerLocationId = transferDb.CustomerLocationId,
                                InventoryId = transferDb.Item.Inventory.InventoryId,
                                BinId = binDb.BinId,
                                Qty = 0,
                                LotId = bin.LotId
                            };
                            _context.Add(newInventoryBin);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            if (newInventoryBin.LotId != bin.LotId)
                            {
                                await transaction.RollbackAsync();

                                if (newInventoryBin.LotId is not null
                                    || bin.LotId is not null)
                                {
                                    var lotFrom = bin.LotId is not null ? bin.Lot.LotNo : "NO LOT NUMBER";
                                    var lotTo = newInventoryBin.LotId is not null ? newInventoryBin.Lot.LotNo : "NO LOT NUMBER";

                                    return Result.Fail($"Cannot transfer item with different lot number in the same location. Item's lot number: FROM: '{lotFrom}', TO: '{lotTo}'.");
                                }
                            }
                        }

                        var newInventoryBinQty = newInventoryBin.Qty;

                        // update inventory qty, zone qty and bin qty
                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == transferDb.ItemId);

                        var oldQty = inventory.QtyOnHand;

                        // decrease qty of current bin and zone
                        currentInventoryBin.Qty = currentInventoryBinQty - model.QtyTransfered.Value;
                        await _context.SaveChangesAsync();

                        // delete current if zero
                        if (currentInventoryBin.Qty.IsZero())
                        {
                            currentInventoryBin.Deleted = true;
                            await _context.SaveChangesAsync();
                        }

                        // sum all bins of selected zoneId
                        var currentInventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.ZoneId == transferDb.TransferCurrent.CurrentZoneId);

                        var sumCurrentInventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == transferDb.ItemId
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

                        // increase qty of new bin  and zone
                        newInventoryBin.Qty = newInventoryBinQty + model.QtyTransfered.Value;
                        await _context.SaveChangesAsync();

                        // sum all bins of selected zoneId
                        var newInventoryZone = await _context.InventoryZones
                            .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                && x.ZoneId == model.NewBinGetCreate.ZoneId);

                        if (newInventoryZone is null)
                        {
                            newInventoryZone = new InventoryZone
                            {
                                CustomerLocationId = transferDb.CustomerLocationId,
                                InventoryId = inventory.InventoryId,
                                ZoneId = model.NewBinGetCreate.ZoneId,
                                Qty = 0
                            };
                            _context.Add(newInventoryZone);
                            await _context.SaveChangesAsync();
                        }

                        var sumNewInventoryBinQty = await _context.InventoryBins
                            .Include(x => x.Inventory)
                            .Include(x => x.Bin)
                            .Where(x => x.Inventory.ItemId == transferDb.ItemId
                                && x.Bin.ZoneId == newInventoryZone.ZoneId)
                            .SumAsync(x => x.Qty);

                        newInventoryZone.Qty = sumNewInventoryBinQty;
                        await _context.SaveChangesAsync();

                        // sum all zones of selected inventory
                        var inventoryZoneQty = await _context.InventoryZones
                            .Include(x => x.Inventory)
                            .Where(x => x.Inventory.ItemId == transferDb.ItemId)
                            .SumAsync(x => x.Qty);

                        // sum selected inventory in pallet
                        var inventoryPalletQty = await _context.PalletInventories
                            .Include(x => x.Inventory)
                            .Where(x => x.Inventory.ItemId == transferDb.ItemId)
                            .SumAsync(x => x.Qty);

                        var inventoryQty = inventoryZoneQty + inventoryPalletQty;

                        inventory.QtyOnHand = inventoryQty;

                        await _context.SaveChangesAsync();

                        // create log
                        var activityLog = new ActivityLog
                        {
                            CustomerId = state.CustomerId,
                            Type = ActivityLogTypeEnum.Transfer,
                            InventoryId = inventory.InventoryId,
                            UserId = model.UserId,
                            ActivityDateTime = DateTime.Now,
                            Qty = model.QtyTransfered.Value,
                            OldQty = oldQty,
                            NewQty = inventoryQty,
                            MathematicalSymbol = null,
                            ZoneId = model.NewBinGetCreate.ZoneId,
                            BinId = binDb.BinId
                        };
                        _context.Add(activityLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new TransferGetModel
                        {
                            TransferId = transferDb.TransferId,
                            CustomerLocationId = transferDb.CustomerLocationId.Value,
                            ItemId = transferDb.ItemId.Value,
                            TransferCurrentId = transferDb.TransferCurrentId.Value,
                            TransferNewId = transferDb.TransferNewId,
                            Qty = transferDb.Qty,
                            Remaining = transferDb.Remaining,
                            Status = transferDb.Status,
                            TransferDateTime = transferDb.TransferDateTime,
                            ReceivedDateTime = transferDb.CompletedDateTime
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