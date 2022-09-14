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
    public class BinService : IBinService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public BinService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<BinGetModel>> CreateBinAsync(AppState state, BinCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(BinCreateModel.CustomerLocationId)} is required.");
                }

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

                if (model.ZoneId is null)
                {
                    return Result.Fail($"{nameof(BinCreateModel.ZoneId)} is required.");
                }
                else
                {
                    var exist = await _context.Zones
                        .AnyAsync(x => x.ZoneId == model.ZoneId
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Zone)} not found.");
                    }
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(BinCreateModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Bins
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.CustomerLocationId == model.CustomerLocationId
                            && x.ZoneId == model.ZoneId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(BinCreateModel.Name)} is already exist.");
                    }
                }

                var entity = new Bin
                {
                    CustomerLocationId = model.CustomerLocationId,
                    ZoneId = model.ZoneId,
                    Name = model.Name,
                    Category = model.Category,
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new BinGetModel
                {
                    BinId = entity.BinId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    ZoneId = entity.ZoneId.Value,
                    Name = entity.Name,
                    Category = entity.Category,
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<PurchaseOrderImportModel>> AddImportedZonesAndBins(AppState state, int customerLocationId, ZoneAndBinImportModel[] model)
        {
            try
            {
                Bin currentBin = new Bin();  // keeps track of which bin we are currently trying to save and import.
                Zone currentZone = new Zone(); // keeps track of current zone we are trying to save and import.
                List<string> uniqueZones = new List<string>();
                List<string> uniqueBins = new List<string>();
                // Terminating early on duplicates for any import entry

                foreach (ZoneAndBinImportModel zone in model)
                {
                    Zone existingZone = await _context.Zones.FirstOrDefaultAsync(z => z.Name == zone.ZoneName
                                                                                                    && z.CustomerLocationId == customerLocationId
                                                                                                    && z.CustomerFacilityId == zone.CustomerFacilityId);
                    if (existingZone != null)
                    {
                        return Result.Fail($"Zone: {existingZone.Name}, already exists in this facility. Import was terminated");
                    }
                    if (!uniqueZones.Contains(zone.ZoneName))
                    {
                        uniqueZones.Add(zone.ZoneName);

                        Zone zoneEntity = new Zone()
                        {
                            CustomerLocationId = customerLocationId,
                            CustomerFacilityId = zone.CustomerFacilityId,
                            Name = zone.ZoneName,
                        };

                        await _context.Zones.AddAsync(zoneEntity);
                        await _context.SaveChangesAsync();

                        currentZone = zoneEntity;
                    }
                }

                foreach (ZoneAndBinImportModel bin in model)
                {
                    Zone binZone = await _context.Zones.FirstOrDefaultAsync(z => z.Name == bin.BinZone);

                    Bin existingBin = await _context.Bins.FirstOrDefaultAsync(b => b.Name == bin.BinName
                                                                                                    && b.CustomerLocationId == customerLocationId
                                                                                                    && b.ZoneId == binZone.ZoneId);
                    if (existingBin != null)
                    {
                        return Result.Fail($"Bin: {existingBin.Name}, already exists in this zone. Import was terminated");
                    }

                    if (!uniqueBins.Contains(bin.BinName))
                    {
                        uniqueBins.Add(bin.BinName);

                        Bin binEntity = new Bin()
                        {
                            CustomerLocationId = bin.CustomerLocationId,
                            Name = bin.BinName,
                            ZoneId = binZone.ZoneId,
                            Category = bin.Category,
                        };

                        await _context.Bins.AddAsync(binEntity);
                        await _context.SaveChangesAsync();

                        currentBin = binEntity;
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

        public async Task<Result<BinGetModel>> EditBinAsync(AppState state, BinEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.BinId is null)
                {
                    return Result.Fail($"{nameof(BinEditModel.BinId)} is required.");
                }

                Bin entity;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    entity = await _context.Bins
                        .Include(x => x.CustomerLocation)
                        .SingleOrDefaultAsync(x => x.BinId == model.BinId
                            && x.CustomerLocation.CustomerId == state.CustomerId);
                }
                else
                {
                    entity = await _context.Bins
                        .SingleOrDefaultAsync(x => x.BinId == model.BinId);
                }

                if (entity is null)
                {
                    return Result.Fail($"{nameof(Bin)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(BinEditModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Bins
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.Name.Trim().ToLower() != entity.Name.Trim().ToLower()
                            && x.CustomerLocationId == entity.CustomerLocationId
                            && x.ZoneId == entity.ZoneId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(BinEditModel.Name)} is already exist.");
                    }
                }

                entity.Name = model.Name;
                await _context.SaveChangesAsync();

                return Result.Ok(new BinGetModel
                {
                    BinId = entity.BinId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    ZoneId = entity.ZoneId.Value,
                    Name = entity.Name,
                    Category = entity.Category,
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinGetModel>>> GetBinByCustomerLocationIdAsync(AppState state, int customerLocationId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.CustomerLocations.AsQueryable();

                if (state.Role == RoleEnum.SuperAdmin)
                {
                    query = query
                        .Where(x => x.CustomerLocationId == customerLocationId);
                }
                else
                {
                    query = query
                        .Where(x => x.CustomerLocationId == customerLocationId
                            && x.CustomerId == state.CustomerId);
                }

                var customerLocationExist = await query
                    .AnyAsync();

                if (!customerLocationExist)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                IEnumerable<BinGetModel> model = await _context.Bins
                    .AsNoTracking()
                    .Select(x => new BinGetModel
                    {
                        BinId = x.BinId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        ZoneId = x.ZoneId.Value,
                        ZoneName = x.Zone.Name,
                        Name = x.Name,
                        Category = x.Category,
                    })
                    .Where(x => x.CustomerLocationId == customerLocationId)
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<BinGetModel>> GetBinAsync(AppState state, int binId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var exist = await _context.Bins
                        .Include(x => x.CustomerLocation)
                        .AnyAsync(x => x.BinId == binId
                            && x.CustomerLocation.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Bin)} not found.");
                    }
                }

                var model = await _context.Bins
                    .AsNoTracking()
                    .Select(x => new BinGetModel
                    {
                        BinId = x.BinId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        ZoneId = x.ZoneId.Value,
                        ZoneName = x.Zone.Name,
                        Name = x.Name,
                        Category = x.Category,
                    })
                    .SingleOrDefaultAsync(x => x.BinId == binId);

                if (model is null)
                {
                    return Result.Fail($"{nameof(Bin)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinLookupGetModel>>> GetBinLookupAsync(AppState state, int zoneId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Bins
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.ZoneId == zoneId
                        && x.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.ZoneId == zoneId);
                }

                IEnumerable<BinLookupGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new BinLookupGetModel
                    {
                        BinId = x.BinId,
                        Name = x.Name
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Bin)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinLookupItemQuantityGetModel>>> GetBinLookupItemQuantityAsync(AppState state, int itemId, int zoneId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Bin)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.Inventory.ItemId == itemId
                        && x.Bin.ZoneId == zoneId
                        && x.Bin.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.Inventory.ItemId == itemId
                        && x.Bin.ZoneId == zoneId);
                }

                IEnumerable<BinLookupItemQuantityGetModel> model = await query
                    .AsNoTracking()
                    .GroupBy(x => x.BinId)
                    .Select(z => new BinLookupItemQuantityGetModel
                    {
                        BinId = z.Key.Value,
                        Name = z.FirstOrDefault().Bin.Name,
                        Qty = z.Sum(xx => xx.Qty)
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Bin)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<BinStorageManagementGetModel>> GetBinStorageManagementAsync(AppState state, int customerLocationId, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var bins = await _context.Bins
                    .Include(x => x.Zone)
                    .Include(x => x.InventoryBins)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.UnitOfMeasure)
                    .AsNoTracking()
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.Zone.CustomerFacilityId == customerFacilityId)
                    .ToListAsync();

                var model = new BinStorageManagementGetModel();
                model.Bins = bins.Count;
                model.Zones = bins.Select(x => x.ZoneId).Distinct().Count();

                var dto = new List<BinStorageManagementGetModel.BinDetail>();
                foreach (var x in bins)
                {
                    var z = new BinStorageManagementGetModel.BinDetail();
                    z.BinId = x.BinId;
                    z.Name = x.Name;
                    z.Category = x.Category;
                    z.Zone = x.Zone.Name;
                    z.Qty = x.InventoryBins.Sum(zz => zz.Qty);

                    var uoms = new List<string>();
                    foreach (var i in x.InventoryBins.Select(zz => zz.Inventory.Item.UnitOfMeasure.Code).Distinct())
                    {
                        uoms.Add(i);
                    }

                    z.UOM = string.Join(", ", uoms);
                    z.UOM = z.UOM.Trim();

                    z.UniqueSKU = x.InventoryBins.Select(zz => zz.Inventory.Item.SKU).Distinct().Count();

                    dto.Add(z);
                }
                model.BinDetails = dto;

                if (model is null)
                {
                    return Result.Fail($"{nameof(Bin)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<BinStorageManagementDetailGetModel>> GetBinStorageManagementDetailAsync(AppState state, int binId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var bin = await _context.Bins
                    .Include(x => x.InventoryBins)
                        .ThenInclude(x => x.Inventory)
                            .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.UnitOfMeasure)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BinId == binId);

                var model = new BinStorageManagementDetailGetModel();
                model.Items = bin.InventoryBins.Select(x => x.InventoryId).Distinct().ToList().Count;
                model.UniqueSKUs = bin.InventoryBins.Select(x => x.Inventory.Item.SKU).Distinct().ToList().Count;
                model.ItemDetails = bin.InventoryBins.Select(x => new BinStorageManagementDetailGetModel.Item
                {
                    ItemId = x.Inventory.Item.ItemId,
                    ItemSKU = x.Inventory.Item.SKU,
                    Description = x.Inventory.Item.Description,
                    UOM = x.Inventory.Item.UnitOfMeasure.Code,
                    Qty = x.Qty
                });

                if (model is null)
                {
                    return Result.Fail($"{nameof(Bin)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinZoneDeviceGetModel>>> GetBinZoneDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var model = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Bin)
                        .ThenInclude(z => z.Zone)
                    .Include(x => x.Lot)
                    .Where(x => x.Inventory.ItemId == itemId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.Bin.Zone.CustomerFacility.CustomerFacilityId == customerFacilityId)
                    .AsNoTracking()
                    .Select(x => new BinZoneDeviceGetModel
                    {
                        ZoneId = x.Bin.Zone.ZoneId,
                        Zone = x.Bin.Zone.Name,
                        BinId = x.Bin.BinId,
                        Bin = x.Bin.Name,
                        Qty = x.Qty,
                        LotNo = x.Lot.LotNo,
                        ExpirationDate = x.Lot.ExpirationDate
                    })
                    .ToListAsync();

                var pi = await _context.PalletInventories
                    .Include(x => x.Pallet)
                        .ThenInclude(x => x.Bin)
                            .ThenInclude(x => x.Zone)
                    .Include(x => x.LicensePlateItem)
                        .ThenInclude(x => x.Lot)
                    .AsNoTracking()
                    .Where(x => x.Inventory.ItemId == itemId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerFacilityId == customerFacilityId)
                    .ToListAsync();

                foreach (var x in pi)
                {
                    model.Add(new BinZoneDeviceGetModel
                    {
                        ZoneId = x.Pallet.Bin.Zone.ZoneId,
                        Zone = x.Pallet.Bin.Zone.Name,
                        BinId = x.Pallet.Bin.BinId,
                        Bin = x.Pallet.Bin.Name,
                        Qty = x.Qty,
                        LotNo = x.LicensePlateItem.Lot?.LotNo,
                        ExpirationDate = x.LicensePlateItem.Lot?.ExpirationDate
                    });
                }

                if (model is null)
                {
                    return Result.Fail($"Location not found.");
                }

                return Result.Ok(model.AsEnumerable());
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinLookupDeviceGetModel>>> GetBinLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Bins
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.ZoneId == zoneId
                        && x.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.ZoneId == zoneId);
                }

                IEnumerable<BinLookupDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new BinLookupDeviceGetModel
                    {
                        BinId = x.BinId,
                        Name = x.Name
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"Location not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinLookupItemQuantityLotDeviceGetModel>>> GetBinItemQuantityLotLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, int itemId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Bins
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.ZoneId == zoneId
                        && x.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.ZoneId == zoneId);
                }

                var bin = await query
                    .AsNoTracking()
                    .Select(x => new
                    {
                        BinId = x.BinId,
                        Name = x.Name
                    })
                    .ToListAsync();

                var query2 = _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                    .Include(x => x.Lot)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query2 = query2.Where(x => x.Inventory.ItemId == itemId
                        && x.Bin.ZoneId == zoneId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.Bin.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query2 = query2.Where(x => x.Inventory.ItemId == itemId
                        && x.Bin.ZoneId == zoneId
                        && x.CustomerLocationId == state.CustomerLocationId);
                }

                var binQtyLot = await query2
                    .AsNoTracking()
                    .GroupBy(x => x.BinId)
                    .Select(z => new
                    {
                        BinId = z.Key.Value,
                        Qty = z.Sum(xx => xx.Qty),
                        LotNo = z.FirstOrDefault().Lot.LotNo,
                        ExpirationDate = z.FirstOrDefault().Lot != null
                            ? z.FirstOrDefault().Lot.ExpirationDate
                            : default(DateTime?)
                    })
                    .ToListAsync();

                var model = new List<BinLookupItemQuantityLotDeviceGetModel>();
                foreach (var x in bin)
                {
                    var dto = new BinLookupItemQuantityLotDeviceGetModel();
                    dto.BinId = x.BinId;
                    dto.Name = x.Name;

                    if (binQtyLot.Any(z => z.BinId == x.BinId))
                    {
                        var b = binQtyLot.SingleOrDefault(z => z.BinId == x.BinId);

                        if (b.Qty > 0)
                        {
                            dto.Qty = b.Qty;
                        }

                        dto.LotNo = b.LotNo;
                        dto.ExpirationDate = b.ExpirationDate;
                    }

                    model.Add(dto);
                }

                return Result.Ok(model.AsEnumerable());
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinLookupItemQuantityGetModel>>> GetBinLookupItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, int zoneId, string searchText, bool barcodeSearch = false)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (barcodeSearch && !searchText.HasValue())
                {
                    return Result.Fail("Name is required.");
                }

                var query = _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                    .Include(x => x.Lot)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (barcodeSearch)
                    {
                        query = query.Where(x => x.Inventory.ItemId == itemId
                            && x.Bin.ZoneId == zoneId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.Bin.Zone.CustomerFacilityId == customerFacilityId
                            && x.Bin.Name.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => x.Inventory.ItemId == itemId
                            && x.Bin.ZoneId == zoneId
                            && x.CustomerLocationId == state.CustomerLocationId
                            && x.Bin.Zone.CustomerFacilityId == customerFacilityId
                            && x.Bin.Name.Trim().ToLower().Contains(searchText));
                    }
                }
                else
                {
                    query = query.Where(x => x.Inventory.ItemId == itemId
                        && x.Bin.ZoneId == zoneId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.Bin.Zone.CustomerFacilityId == customerFacilityId);
                }

                IEnumerable<BinLookupItemQuantityGetModel> model = await query
                    .AsNoTracking()
                    .GroupBy(x => new { x.BinId, x.LotId })
                    .Select(z => new BinLookupItemQuantityGetModel
                    {
                        BinId = z.Key.BinId.Value,
                        Name = z.FirstOrDefault().Bin.Name,
                        Qty = z.Sum(xx => xx.Qty),
                        LotNo = z.FirstOrDefault().Lot.LotNo,
                        ExpirationDate = z.FirstOrDefault().Lot.ExpirationDate
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"Location not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<BinLookupItemQuantityGetModel>> GetBinItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, int binId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var model = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Bin)
                    .Where(x => x.Inventory.ItemId == itemId
                        && x.BinId == binId
                        && x.CustomerLocationId == state.CustomerLocationId)
                    .AsNoTracking()
                    .GroupBy(x => x.BinId)
                    .Select(z => new BinLookupItemQuantityGetModel
                    {
                        BinId = z.Key.Value,
                        Name = z.FirstOrDefault().Bin.Name,
                        Qty = z.Sum(xx => xx.Qty)
                    })
                    .SingleOrDefaultAsync();

                if (model is null)
                {
                    return Result.Fail($"Location not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<BinLookupOptionalPalletDeviceGetModel>>> GetBinLookupOptionalPalletDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Bins
                    .Include(x => x.Pallets)
                    .Include(x => x.InventoryBins)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.ZoneId == zoneId
                        && x.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.ZoneId == zoneId);
                }

                IEnumerable<BinLookupOptionalPalletDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new BinLookupOptionalPalletDeviceGetModel
                    {
                        BinId = x.BinId,
                        Name = x.Name,
                        PalletCount = x.Pallets.Count == 0 ? default(int?) : x.Pallets.Count,
                        EachCount = x.InventoryBins.Count == 0 ? default(int?) : x.InventoryBins.Count,
                        PalletQty = x.Pallets.Count == 0 ? default(int?) : x.Pallets.Sum(z => z.Qty),
                        EachQty = x.InventoryBins.Count == 0 ? default(int?) : x.InventoryBins.Sum(z => z.Qty)
                    })
                    .ToListAsync();

                foreach (var x in model)
                {
                    x.TotalQty = x.PalletQty + x.EachQty;
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