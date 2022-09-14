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
    public class ZoneService : IZoneService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public ZoneService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<ZoneGetModel>> CreateZoneAsync(AppState state, ZoneCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(ZoneCreateModel.CustomerLocationId)} is required.");
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

                if (model.CustomerFacilityId is null)
                {
                    return Result.Fail($"{nameof(ZoneCreateModel.CustomerFacilityId)} is required.");
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

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(ZoneCreateModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Zones
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerFacilityId == model.CustomerFacilityId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(ZoneCreateModel.Name)} is already exist.");
                    }
                }

                var entity = new Zone
                {
                    CustomerLocationId = model.CustomerLocationId,
                    CustomerFacilityId = model.CustomerFacilityId,
                    Name = model.Name
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new ZoneGetModel
                {
                    ZoneId = entity.ZoneId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    CustomerFacilityId = entity.CustomerFacilityId.Value,
                    Name = entity.Name
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ZoneGetModel>> EditZoneAsync(AppState state, ZoneEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.ZoneId is null)
                {
                    return Result.Fail($"{nameof(ZoneEditModel.ZoneId)} is required.");
                }

                Zone entity;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    entity = await _context.Zones
                        .Include(x => x.CustomerLocation)
                        .SingleOrDefaultAsync(x => x.ZoneId == model.ZoneId
                            && x.CustomerLocation.CustomerId == state.CustomerId);
                }
                else
                {
                    entity = await _context.Zones
                        .SingleOrDefaultAsync(x => x.ZoneId == model.ZoneId);
                }

                if (entity is null)
                {
                    return Result.Fail($"{nameof(Zone)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(ZoneEditModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Zones
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.Name.Trim().ToLower() != entity.Name.Trim().ToLower()
                            && x.CustomerLocationId == entity.CustomerLocationId
                            && x.CustomerFacilityId == entity.CustomerFacilityId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(ZoneEditModel.Name)} is already exist.");
                    }
                }

                entity.Name = model.Name;
                await _context.SaveChangesAsync();

                return Result.Ok(new ZoneGetModel
                {
                    ZoneId = entity.ZoneId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    CustomerFacilityId = entity.CustomerFacilityId.Value,
                    Name = entity.Name
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<ZoneGetModel>> DeleteZoneAsync(AppState state, ZoneDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.ZoneId is null)
                {
                    return Result.Fail($"{nameof(ZoneDeleteModel.ZoneId)} is required.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Zone entity;

                        if (state.Role != RoleEnum.SuperAdmin)
                        {
                            entity = await _context.Zones
                                .Include(x => x.CustomerLocation)
                                .Include(x => x.InventoryZones)
                                // Bins
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.InventoryBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.PutAwayBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.TransferCurrents)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.TransferNews)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.AdjustBinQuantities)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.OrderLineBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.RecallBins)
                                .Include(x => x.ActivityLogs)
                                .Include(x => x.TransferCurrents)
                                    .ThenInclude(x => x.Transfers)
                                        .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.TransferNews)
                                    .ThenInclude(x => x.Transfers)
                                        .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.TransferZoneBins)
                                .AsSplitQuery()
                                .SingleOrDefaultAsync(x => x.ZoneId == model.ZoneId
                                    && x.CustomerLocation.CustomerId == state.CustomerId);
                        }
                        else
                        {
                            entity = await _context.Zones
                                .Include(x => x.CustomerLocation)
                                .Include(x => x.InventoryZones)
                                // Bins
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.InventoryBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.PutAwayBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.TransferCurrents)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.TransferNews)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.AdjustBinQuantities)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.OrderLineBins)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.RecallBins)
                                .Include(x => x.ActivityLogs)
                                .Include(x => x.TransferCurrents)
                                    .ThenInclude(x => x.Transfers)
                                        .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.TransferNews)
                                    .ThenInclude(x => x.Transfers)
                                        .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.TransferZoneBins)
                                .AsSplitQuery()
                                .SingleOrDefaultAsync(x => x.ZoneId == model.ZoneId);
                        }

                        if (entity is null)
                        {
                            return Result.Fail($"{nameof(Zone)} not found.");
                        }

                        entity.Deleted = true;

                        foreach (var x in entity.InventoryZones)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Bins)
                        {
                            x.Deleted = true;

                            foreach (var z in x.InventoryBins)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.ActivityLogs)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.PutAwayBins)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.TransferCurrents)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.TransferNews)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.AdjustBinQuantities)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.TransferZoneBins)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.OrderLineBins)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.RecallBins)
                            {
                                z.Deleted = true;
                            }
                        }

                        foreach (var x in entity.ActivityLogs)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.ActivityLogs)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.TransferCurrents)
                        {
                            x.Deleted = true;

                            foreach (var z in x.Transfers)
                            {
                                z.Deleted = true;

                                foreach (var zz in z.TransferZoneBins)
                                {
                                    zz.Deleted = true;
                                }
                            }
                        }

                        foreach (var x in entity.TransferNews)
                        {
                            x.Deleted = true;

                            foreach (var z in x.Transfers)
                            {
                                z.Deleted = true;

                                foreach (var zz in z.TransferZoneBins)
                                {
                                    zz.Deleted = true;
                                }
                            }
                        }

                        foreach (var x in entity.TransferZoneBins)
                        {
                            x.Deleted = true;
                        }

                        await _context.SaveChangesAsync();

                        // sum all zones of selected inventory
                        var inventories = await _context.Inventories
                            .Include(x => x.InventoryZones)
                            .Where(x => x.CustomerId == entity.CustomerLocation.CustomerId)
                            .ToListAsync();

                        foreach (var x in inventories)
                        {
                            x.QtyOnHand = x.InventoryZones.Where(x => !x.Deleted).Sum(z => z.Qty);
                        }
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new ZoneGetModel
                        {
                            ZoneId = entity.ZoneId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            CustomerFacilityId = entity.CustomerFacilityId.Value,
                            Name = entity.Name
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

        public async Task<Result<IEnumerable<ZoneGetModel>>> GetZoneByCustomerLocationIdAsync(AppState state, int customerLocationId)
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

                IEnumerable<ZoneGetModel> model = await _context.Zones
                    .AsNoTracking()
                    .Select(x => new ZoneGetModel
                    {
                        ZoneId = x.ZoneId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        CustomerFacilityId = x.CustomerFacilityId.Value,
                        Name = x.Name
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

        public async Task<Result<ZoneGetModel>> GetZoneAsync(AppState state, int zoneId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var exist = await _context.Zones
                        .Include(x => x.CustomerLocation)
                        .AnyAsync(x => x.ZoneId == zoneId
                            && x.CustomerLocation.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Zone)} not found.");
                    }
                }

                var model = await _context.Zones
                    .AsNoTracking()
                    .Select(x => new ZoneGetModel
                    {
                        ZoneId = x.ZoneId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        CustomerFacilityId = x.CustomerFacilityId.Value,
                        Name = x.Name
                    })
                    .SingleOrDefaultAsync(x => x.ZoneId == zoneId);

                if (model is null)
                {
                    return Result.Fail($"{nameof(Zone)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ZoneLookupGetModel>>> GetZoneLookupAsync(AppState state, int customerFacilityId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Zones
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerFacilityId == customerFacilityId
                        && x.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerFacilityId == customerFacilityId);
                }

                IEnumerable<ZoneLookupGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ZoneLookupGetModel
                    {
                        ZoneId = x.ZoneId,
                        Name = x.Name
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Zone)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ZoneInventoryGetModel>>> GetZoneInventoryDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int inventoryId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                IEnumerable<ZoneInventoryGetModel> model = await _context.InventoryZones
                    .Include(x => x.Zone)
                        .ThenInclude(x => x.CustomerFacility)
                    .Include(z => z.Zone)
                    .Where(x => x.InventoryId == inventoryId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.Zone.CustomerFacility.CustomerFacilityId == customerFacilityId)
                    .AsNoTracking()
                    .Select(x => new ZoneInventoryGetModel
                    {
                        ZoneId = x.Zone.ZoneId,
                        Name = x.Zone.Name,
                        Qty = x.Qty
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"Area not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ZoneLookupDeviceGetModel>>> GetZoneLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Zones
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerFacilityId == customerFacilityId
                        && x.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId
                        && x.CustomerFacilityId == customerFacilityId);
                }

                IEnumerable<ZoneLookupDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ZoneLookupDeviceGetModel
                    {
                        ZoneId = x.ZoneId,
                        Name = x.Name
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"Area not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<ZoneLookupItemQuantityGetModel>>> GetZoneLookupItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.InventoryZones
                    .Include(x => x.Zone)
                    .Include(x => x.Inventory)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.Inventory.ItemId == itemId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.Zone.CustomerFacilityId == customerFacilityId
                        && x.Zone.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.Inventory.ItemId == itemId
                        && x.CustomerLocationId == state.CustomerLocationId
                        && x.Zone.CustomerFacilityId == customerFacilityId);
                }

                IEnumerable<ZoneLookupItemQuantityGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new ZoneLookupItemQuantityGetModel
                    {
                        ZoneId = x.Zone.ZoneId,
                        Name = x.Zone.Name,
                        Qty = x.Qty
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"Area not found.");
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