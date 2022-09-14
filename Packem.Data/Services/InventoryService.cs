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
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public InventoryService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<InventoryDropDownGetModel>> GetInventoryDropDownAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var uomDb = await _context.UnitOfMeasureCustomers
                    .Include(x => x.UnitOfMeasure)
                    .Where(x => x.CustomerId == customerId)
                    .ToListAsync();

                if (uomDb is null)
                {
                    return Result.Fail($"{nameof(UnitOfMeasure)} is empty.");
                }

                var uoms = new List<InventoryDropDownGetModel.UnitOfMeasure>();
                foreach (var x in uomDb)
                {
                    uoms.Add(new InventoryDropDownGetModel.UnitOfMeasure
                    {
                        Code = x.UnitOfMeasure.Code,
                        Description = x.UnitOfMeasure.Description
                    });
                }

                var queryCustomerLocations = _context.CustomerLocations.AsQueryable();
                if (state.Role != RoleEnum.SuperAdmin)
                {
                    queryCustomerLocations = queryCustomerLocations
                        .Where(x => x.CustomerId == state.CustomerId);
                }
                var customerLocations = await queryCustomerLocations
                    .Select(x => new InventoryDropDownGetModel.CustomerLocation
                    {
                        CustomerLocationId = x.CustomerLocationId,
                        Name = x.Name
                    })
                    .AsNoTracking()
                    .ToListAsync();

                var queryCustomerFacilities = _context.CustomerFacilities.AsQueryable();
                if (state.Role != RoleEnum.SuperAdmin)
                {
                    queryCustomerFacilities = queryCustomerFacilities
                        .Where(x => x.CustomerId == state.CustomerId);
                }
                var customerFacilities = await queryCustomerFacilities
                    .Select(x => new InventoryDropDownGetModel.CustomerFacility
                    {
                        CustomerFacilityId = x.CustomerFacilityId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        Name = x.Name
                    })
                    .AsNoTracking()
                    .ToListAsync();

                var queryZones = _context.Zones
                    .Include(x => x.CustomerLocation)
                        .ThenInclude(x => x.Customer)
                    .AsQueryable();
                if (state.Role != RoleEnum.SuperAdmin)
                {
                    queryZones = queryZones
                        .Where(x => x.CustomerLocation.Customer.CustomerId == state.CustomerId);
                }
                var zones = await queryZones
                    .Select(x => new InventoryDropDownGetModel.Zone
                    {
                        ZoneId = x.ZoneId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        CustomerFacilityId = x.CustomerFacilityId.Value,
                        Name = x.Name
                    })
                    .AsNoTracking()
                    .ToListAsync();

                var queryBins = _context.Bins
                    .Include(x => x.CustomerLocation)
                        .ThenInclude(x => x.Customer)
                    .AsQueryable();
                if (state.Role != RoleEnum.SuperAdmin)
                {
                    queryBins = queryBins
                        .Where(x => x.CustomerLocation.Customer.CustomerId == state.CustomerId);
                }
                var bins = await queryBins
                    .Select(x => new InventoryDropDownGetModel.Bin
                    {
                        BinId = x.BinId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        ZoneId = x.ZoneId.Value,
                        Name = x.Name
                    })
                    .AsNoTracking()
                    .ToListAsync();
                bins.Insert(0, new InventoryDropDownGetModel.Bin
                {
                    BinId = null,
                    CustomerLocationId = null,
                    ZoneId = null,
                    Name = "New bin location"
                });

                var queryVendors = _context.Vendors
                    .Include(x => x.Customer)
                    .AsQueryable();
                if (state.Role != RoleEnum.SuperAdmin)
                {
                    queryVendors = queryVendors
                        .Where(x => x.CustomerId == state.CustomerId);
                }
                var vendors = await queryVendors
                    .Select(x => new InventoryDropDownGetModel.Vendor
                    {
                        VendorId = x.VendorId,
                        CustomerId = x.CustomerId,
                        VendorNo = x.VendorNo,
                        Name = x.Name,
                        PointOfContact = x.PointOfContact,
                        Address = x.Address1,
                        PhoneNumber = x.PhoneNumber
                    })
                    .AsNoTracking()
                    .ToListAsync();
                vendors.Insert(0, new InventoryDropDownGetModel.Vendor
                {
                    VendorId = null,
                    CustomerId = null,
                    VendorNo = null,
                    Name = "New Vendor",
                    PointOfContact = null,
                    Address = null,
                    PhoneNumber = null
                });

                var model = new InventoryDropDownGetModel();
                model.UnitOfMeasures = uoms;
                model.CustomerLocations = customerLocations;
                model.CustomerFacilities = customerFacilities;
                model.Zones = zones;
                model.Bins = bins;
                model.Vendors = vendors;

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<InventoryGetModel>> CreateInventoryAsync(AppState state, InventoryCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(InventoryCreateModel.CustomerId)} is required.");
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

                Item item = await _context.Items
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.CustomerId == model.CustomerId
                        && x.ItemId == model.ItemId);

                if (item is null)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(InventoryCreateModel.CustomerLocationId)} is required.");
                }
                else
                {
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

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail("Location not found.");
                    }
                }

                if (model.FacilityId is null)
                {
                    return Result.Fail($"{nameof(InventoryCreateModel.FacilityId)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerFacilities
                        .AnyAsync(x => x.CustomerFacilityId == model.FacilityId
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (!exist)
                    {
                        return Result.Fail("Facility not found.");
                    }
                }

                if (model.QtyAtBin is null)
                {
                    return Result.Fail($"{nameof(InventoryCreateModel.QtyAtBin)} is required.");
                }
                else
                {
                    if (model.QtyAtBin.Value.IsNegative())
                    {
                        return Result.Fail($"{nameof(InventoryCreateModel.QtyAtBin)} cannot be negative.");
                    }
                    else if (model.QtyAtBin.Value.IsZero())
                    {
                        return Result.Fail($"{nameof(InventoryCreateModel.QtyAtBin)} cannot be zero.");
                    }
                }

                if (model.ZoneId is null)
                {
                    return Result.Fail($"{nameof(InventoryCreateModel.ZoneId)} is required.");
                }
                else
                {
                    var exist = await _context.Zones
                        .AnyAsync(x => x.CustomerFacilityId == model.FacilityId
                            && x.ZoneId == model.ZoneId);

                    if (!exist)
                    {
                        return Result.Fail("Area not found.");
                    }
                }

                if (model.BinId is null) // new bin
                {
                    if (!model.BinLocation.HasValue())
                    {
                        return Result.Fail($"Location is required.");
                    }
                    else
                    {
                        var exist = await _context.Bins
                            .AnyAsync(x => x.Name.Trim().ToLower() == model.BinLocation.Trim().ToLower()
                                && x.CustomerLocationId == model.CustomerLocationId
                                && x.ZoneId == model.ZoneId);

                        if (exist)
                        {
                            return Result.Fail("Location is already exist.");
                        }
                    }
                }
                else
                {
                    var exist = await _context.Bins
                        .AnyAsync(x => x.BinId == model.BinId
                            && x.CustomerLocationId == model.CustomerLocationId
                            && x.ZoneId == model.ZoneId);

                    if (!exist)
                    {
                        return Result.Fail("Location not found.");
                    }

                    var palletCheck = await _context.Pallets
                        .AsNoTracking()
                        .AnyAsync(x => x.BinId == model.BinId);

                    if (palletCheck)
                    {
                        return Result.Fail($"Cannot put item in this location. 'Each' and 'Pallet' cannot be in same location.");
                    }
                }

                if (model.VendorId is null) // new vendor
                {
                    if (!model.VendorNo.HasValue())
                    {
                        return Result.Fail($"{nameof(InventoryCreateModel.VendorNo)} is required.");
                    }
                    else
                    {
                        var exist = await _context.Vendors
                            .AnyAsync(x => x.VendorNo.Trim().ToLower() == model.VendorNo.Trim().ToLower()
                                && x.CustomerId == model.CustomerId);

                        if (exist)
                        {
                            return Result.Fail($"{nameof(InventoryCreateModel.VendorNo)} is already exist.");
                        }
                    }

                    if (!model.VendorName.HasValue())
                    {
                        return Result.Fail($"{nameof(InventoryCreateModel.VendorName)} is required.");
                    }
                    else
                    {
                        var exist = await _context.Vendors
                            .AnyAsync(x => x.Name.Trim().ToLower() == model.VendorName.Trim().ToLower()
                                && x.CustomerId == model.CustomerId);

                        if (exist)
                        {
                            return Result.Fail($"{nameof(InventoryCreateModel.VendorName)} is already exist.");
                        }
                    }

                    if (!model.VendorPointOfContact.HasValue())
                    {
                        return Result.Fail($"{nameof(InventoryCreateModel.VendorPointOfContact)} is required.");
                    }

                    if (!model.VendorAddress.HasValue())
                    {
                        return Result.Fail($"{nameof(InventoryCreateModel.VendorAddress)} is required.");
                    }

                    if (!model.VendorPhoneNumber.HasValue())
                    {
                        return Result.Fail($"{nameof(InventoryCreateModel.VendorPhoneNumber)} is required.");
                    }
                }
                else
                {
                    var exist = await _context.Vendors
                        .AnyAsync(x => x.VendorId == model.VendorId
                            && x.CustomerId == model.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail("Vendor not found.");
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
                        int vendorId;
                        if (model.VendorId is null) // new vendor
                        {
                            Vendor vendor = new Vendor
                            {
                                CustomerId = model.CustomerId,
                                VendorNo = model.VendorNo,
                                Name = model.VendorName,
                                Address1 = model.VendorAddress,
                                Address2 = null,
                                City = string.Empty,
                                StateProvince = string.Empty,
                                ZipPostalCode = string.Empty,
                                PointOfContact = model.VendorPointOfContact,
                                PhoneNumber = model.VendorPhoneNumber
                            };
                            _context.Add(vendor);
                            await _context.SaveChangesAsync();

                            vendorId = vendor.VendorId;

                            var itemVendor = new ItemVendor
                            {
                                CustomerId = model.CustomerId,
                                ItemId = model.ItemId,
                                VendorId = vendor.VendorId
                            };
                            _context.Add(itemVendor);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            vendorId = model.VendorId.Value;
                        }

                        var itemVendorExist = await _context.ItemVendors
                            .AnyAsync(x => x.ItemId == model.ItemId
                                    && x.VendorId == vendorId);

                        if (!itemVendorExist)
                        {
                            var itemVendor = new ItemVendor
                            {
                                CustomerId = model.CustomerId,
                                ItemId = model.ItemId,
                                VendorId = vendorId
                            };
                            _context.Add(itemVendor);
                            await _context.SaveChangesAsync();
                        }

                        var inventoryExist = false;
                        var inventory = await _context.Inventories
                            .SingleOrDefaultAsync(x => x.ItemId == model.ItemId);

                        var oldQty = 0;
                        var inventoryQty = 0;

                        if (inventory is null) // create
                        {
                            inventory = new Inventory
                            {
                                CustomerId = model.CustomerId,
                                ItemId = item.ItemId,
                                QtyOnHand = model.QtyAtBin.Value // no need to sum, because it is new item
                            };
                            _context.Add(inventory);
                            await _context.SaveChangesAsync();

                            oldQty = model.QtyAtBin.Value;
                            inventoryQty = model.QtyAtBin.Value;
                        }
                        else
                        {
                            inventoryExist = true;
                            oldQty = inventory.QtyOnHand;
                        }

                        var binId = 0;

                        if (model.BinId is null) // create new bin
                        {
                            var bin = new Bin
                            {
                                CustomerLocationId = model.CustomerLocationId,
                                ZoneId = model.ZoneId,
                                Name = model.BinLocation
                            };
                            _context.Add(bin);
                            await _context.SaveChangesAsync();

                            binId = bin.BinId;

                            var inventoryBin = new InventoryBin
                            {
                                CustomerLocationId = model.CustomerLocationId,
                                InventoryId = inventory.InventoryId,
                                BinId = bin.BinId,
                                Qty = model.QtyAtBin.Value,
                                LotId = model.LotId
                            };
                            _context.Add(inventoryBin);
                            await _context.SaveChangesAsync();

                            // sum all bins of selected zoneId
                            var inventoryBinQty = await _context.InventoryBins
                                .Include(x => x.Inventory)
                                .Include(x => x.Bin)
                                .Where(x => x.Inventory.ItemId == model.ItemId
                                    && x.Bin.ZoneId == model.ZoneId)
                                .SumAsync(x => x.Qty);

                            var inventoryZone = await _context.InventoryZones
                                .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                    && x.ZoneId == model.ZoneId);

                            if (inventoryZone is null)
                            {
                                inventoryZone = new InventoryZone
                                {
                                    CustomerLocationId = model.CustomerLocationId,
                                    InventoryId = inventory.InventoryId,
                                    ZoneId = model.ZoneId,
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
                        }
                        else // update selected bin
                        {
                            binId = model.BinId.Value;

                            var inventoryBin = await _context.InventoryBins
                                .Include(x => x.Lot)
                                .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                    && x.BinId == model.BinId
                                    && x.CustomerLocationId == model.CustomerLocationId);

                            if (inventoryBin is null)
                            {
                                inventoryBin = new InventoryBin
                                {
                                    CustomerLocationId = model.CustomerLocationId,
                                    InventoryId = inventory.InventoryId,
                                    BinId = model.BinId,
                                    Qty = model.QtyAtBin.Value,
                                    LotId = model.LotId
                                };
                                _context.Add(inventoryBin);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                if (inventoryBin.LotId != model.LotId)
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

                                inventoryBin.Qty = inventoryBin.Qty + model.QtyAtBin.Value;
                                await _context.SaveChangesAsync();
                            }

                            // sum all bins of selected zoneId
                            var inventoryBinQty = await _context.InventoryBins
                                .Include(x => x.Inventory)
                                .Include(x => x.Bin)
                                .Where(x => x.Inventory.ItemId == model.ItemId
                                    && x.Bin.ZoneId == model.ZoneId)
                                .SumAsync(x => x.Qty);

                            var inventoryZone = await _context.InventoryZones
                                .SingleOrDefaultAsync(x => x.InventoryId == inventory.InventoryId
                                    && x.ZoneId == model.ZoneId);

                            if (inventoryZone is null)
                            {
                                inventoryZone = new InventoryZone
                                {
                                    CustomerLocationId = model.CustomerLocationId,
                                    InventoryId = inventory.InventoryId,
                                    ZoneId = model.ZoneId,
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
                        }

                        if (inventoryExist)
                        {
                            // sum all zones of selected inventory
                            var inventoryZoneQty = await _context.InventoryZones
                                .Include(x => x.Inventory)
                                .Where(x => x.Inventory.ItemId == model.ItemId)
                                .SumAsync(x => x.Qty);

                            // sum selected inventory in pallet
                            var inventoryPalletQty = await _context.PalletInventories
                                .Include(x => x.Inventory)
                                .Where(x => x.Inventory.ItemId == model.ItemId)
                                .SumAsync(x => x.Qty);

                            inventoryQty = inventoryZoneQty + inventoryPalletQty;

                            inventory.QtyOnHand = inventoryQty;

                            await _context.SaveChangesAsync();
                        }

                        // create log
                        var activityLog = new ActivityLog
                        {
                            CustomerId = state.CustomerId,
                            Type = ActivityLogTypeEnum.AddBin,
                            InventoryId = inventory.InventoryId,
                            UserId = state.UserId,
                            ActivityDateTime = DateTime.Now,
                            Qty = model.QtyAtBin.Value,
                            OldQty = oldQty,
                            NewQty = inventoryQty,
                            MathematicalSymbol = MathematicalSymbolEnum.Plus,
                            ZoneId = model.ZoneId,
                            BinId = binId
                        };
                        _context.Add(activityLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new InventoryGetModel
                        {
                            InventoryId = inventory.InventoryId,
                            CustomerLocationId = inventory.CustomerId.Value,
                            ItemId = inventory.ItemId.Value,
                            QtyOnHand = inventory.QtyOnHand
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
