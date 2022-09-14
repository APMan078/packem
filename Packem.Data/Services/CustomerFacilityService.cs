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
    public class CustomerFacilityService : ICustomerFacilityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public CustomerFacilityService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<CustomerFacilityGetModel>> CreateCustomerFacilityAsync(AppState state, CustomerFacilityCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.CustomerId)} is required.");
                }

                var existCustomerId = await _context.Customers
                        .AnyAsync(x => x.CustomerId == model.CustomerId);

                if (!existCustomerId)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.CustomerLocationId)} is required.");
                }

                var customerLocationExist = await _context.CustomerLocations
                        .AnyAsync(x => x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == model.CustomerId);

                if (!customerLocationExist)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerFacilities
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.CustomerLocationId == model.CustomerLocationId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerFacilityCreateModel.Name)} already exists.");
                    }
                }

                if (!model.Address.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.Address)} is required.");
                }

                if (!model.City.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.City)} is required.");
                }

                if (!model.StateProvince.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.StateProvince)} is required.");
                }

                if (!model.ZipPostalCode.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.ZipPostalCode)} is required.");
                }

                if (!model.PhoneNumber.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityCreateModel.PhoneNumber)} is required.");
                }

                var entity = new CustomerFacility
                {
                    CustomerId = model.CustomerId.Value,
                    CustomerLocationId = model.CustomerLocationId,
                    Name = model.Name,
                    Address = model.Address,
                    Address2 = model.Address2,
                    City = model.City,
                    StateProvince = model.StateProvince,
                    ZipPostalCode = model.ZipPostalCode,
                    PhoneNumber = model.PhoneNumber,
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerFacilityGetModel
                {
                    CustomerId = entity.CustomerId,
                    CustomerFacilityId = entity.CustomerFacilityId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    Name = entity.Name,
                    Address = entity.Address,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    PhoneNumber = entity.PhoneNumber,
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerFacilityGetModel>> EditCustomerFacilityAsync(AppState state, CustomerFacilityEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerFacilityId is null)
                {
                    return Result.Fail($"{nameof(CustomerFacilityEditModel.CustomerFacilityId)} is required.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var customerFacilityExist = await _context.CustomerFacilities
                        .Include(x => x.CustomerLocation)
                        .AnyAsync(x => x.CustomerFacilityId == model.CustomerFacilityId
                            && x.CustomerLocation.CustomerId == state.CustomerId);

                    if (!customerFacilityExist)
                    {
                        return Result.Fail($"{nameof(CustomerFacility)} not found.");
                    }
                }

                var entity = await _context.CustomerFacilities
                    .SingleOrDefaultAsync(x => x.CustomerFacilityId == model.CustomerFacilityId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(CustomerFacility)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityEditModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerFacilities
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.Name.Trim().ToLower() != entity.Name.Trim().ToLower()
                            && x.CustomerLocationId == entity.CustomerLocationId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerFacilityEditModel.Name)} is already exist.");
                    }
                }

                if (!model.Address.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityEditModel.Address)} is required.");
                }

                if (!model.City.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityEditModel.City)} is required.");
                }

                if (!model.StateProvince.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityEditModel.StateProvince)} is required.");
                }

                if (!model.ZipPostalCode.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityEditModel.ZipPostalCode)} is required.");
                }

                if (!model.PhoneNumber.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerFacilityEditModel.PhoneNumber)} is required.");
                }


                entity.Name = model.Name;

                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerFacilityGetModel
                {
                    CustomerId = entity.CustomerId,
                    CustomerFacilityId = entity.CustomerFacilityId,
                    CustomerLocationId = entity.CustomerLocationId.Value,
                    Name = entity.Name,
                    Address = entity.Address,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    PhoneNumber = entity.PhoneNumber,
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerFacilityGetModel>> DeleteCustomerFacilityAsync(AppState state, CustomerFacilityDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerFacilityId is null)
                {
                    return Result.Fail($"{nameof(CustomerFacilityDeleteModel.CustomerFacilityId)} is required.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        CustomerFacility entity;

                        if (state.Role != RoleEnum.SuperAdmin)
                        {
                            entity = await _context.CustomerFacilities
                                .Include(x => x.CustomerLocation)
                                .Include(x => x.PurchaseOrders)
                                    .ThenInclude(x => x.Receives)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.InventoryZones)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.InventoryBins)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.PutAwayBins)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.TransferCurrents)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.TransferNews)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.AdjustBinQuantities)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.TransferCurrents)
                                        .ThenInclude(x => x.Transfers)
                                            .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.TransferNews)
                                        .ThenInclude(x => x.Transfers)
                                            .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.SaleOrders)
                                    .ThenInclude(x => x.OrderLines)
                                .AsSplitQuery()
                                .SingleOrDefaultAsync(x => x.CustomerFacilityId == model.CustomerFacilityId
                                    && x.CustomerLocation.CustomerId == state.CustomerId);
                        }
                        else
                        {
                            entity = await _context.CustomerFacilities
                                .Include(x => x.CustomerLocation)
                                .Include(x => x.PurchaseOrders)
                                    .ThenInclude(x => x.Receives)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.InventoryZones)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.InventoryBins)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.PutAwayBins)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.TransferCurrents)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.TransferNews)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.AdjustBinQuantities)
                                .Include(x => x.Zones) // Bins
                                    .ThenInclude(x => x.Bins)
                                        .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.TransferCurrents)
                                        .ThenInclude(x => x.Transfers)
                                            .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.TransferNews)
                                        .ThenInclude(x => x.Transfers)
                                            .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.Zones)
                                    .ThenInclude(x => x.TransferZoneBins)
                                .Include(x => x.SaleOrders)
                                    .ThenInclude(x => x.OrderLines)
                                .AsSplitQuery()
                                .SingleOrDefaultAsync(x => x.CustomerFacilityId == model.CustomerFacilityId);
                        }

                        if (entity is null)
                        {
                            return Result.Fail($"{nameof(CustomerFacility)} not found.");
                        }

                        entity.Deleted = true;

                        foreach (var x in entity.PurchaseOrders)
                        {
                            x.Deleted = true;

                            foreach (var z in x.Receives)
                            {
                                z.Deleted = true;

                                foreach (var xx in z.PutAways)
                                {
                                    xx.Deleted = true;

                                    foreach (var zz in xx.PutAwayBins)
                                    {
                                        zz.Deleted = true;
                                    }
                                }
                            }
                        }

                        foreach (var x in entity.Zones)
                        {
                            x.Deleted = true;

                            foreach (var z in x.InventoryZones)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.Bins)
                            {
                                z.Deleted = true;

                                foreach (var xx in z.InventoryBins)
                                {
                                    xx.Deleted = true;
                                }

                                foreach (var xx in z.ActivityLogs)
                                {
                                    xx.Deleted = true;
                                }

                                foreach (var xx in z.PutAwayBins)
                                {
                                    xx.Deleted = true;
                                }

                                foreach (var xx in z.TransferCurrents)
                                {
                                    xx.Deleted = true;
                                }

                                foreach (var xx in z.TransferNews)
                                {
                                    xx.Deleted = true;
                                }

                                foreach (var xx in z.AdjustBinQuantities)
                                {
                                    xx.Deleted = true;
                                }

                                foreach (var xx in z.TransferZoneBins)
                                {
                                    xx.Deleted = true;
                                }
                            }

                            foreach (var z in x.ActivityLogs)
                            {
                                z.Deleted = true;
                            }

                            foreach (var z in x.TransferCurrents)
                            {
                                z.Deleted = true;

                                foreach (var xx in z.Transfers)
                                {
                                    xx.Deleted = true;

                                    foreach (var zz in xx.TransferZoneBins)
                                    {
                                        zz.Deleted = true;
                                    }
                                }
                            }

                            foreach (var z in x.TransferNews)
                            {
                                z.Deleted = true;

                                foreach (var xx in z.Transfers)
                                {
                                    xx.Deleted = true;

                                    foreach (var zz in xx.TransferZoneBins)
                                    {
                                        zz.Deleted = true;
                                    }
                                }
                            }

                            foreach (var z in x.TransferZoneBins)
                            {
                                z.Deleted = true;
                            }
                        }

                        foreach (var x in entity.SaleOrders)
                        {
                            x.Deleted = true;

                            foreach (var z in x.OrderLines)
                            {
                                z.Deleted = true;
                            }
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

                        return Result.Ok(new CustomerFacilityGetModel
                        {
                            CustomerId = entity.CustomerId,
                            CustomerFacilityId = entity.CustomerFacilityId,
                            CustomerLocationId = entity.CustomerLocationId.Value,
                            Name = entity.Name,
                            Address = entity.Address,
                            Address2 = entity.Address2,
                            City = entity.City,
                            StateProvince = entity.StateProvince,
                            ZipPostalCode = entity.ZipPostalCode,
                            PhoneNumber = entity.PhoneNumber,
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

        public async Task<Result<IEnumerable<CustomerFacilityGetModel>>> GetCustomerFacilitiesByCustomerIdAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var customerExist = await _context.Customers
                        .AnyAsync(x => x.CustomerId == customerId);

                    if (!customerExist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                IEnumerable<CustomerFacilityGetModel> model = await _context.CustomerFacilities
                    .AsNoTracking()
                    .Select(x => new CustomerFacilityGetModel
                    {
                        CustomerId = x.CustomerId,
                        CustomerFacilityId = x.CustomerFacilityId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        Name = x.Name,
                        Address = x.Address,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        PhoneNumber = x.PhoneNumber,
                    })
                    .Where(x => x.CustomerId == customerId)
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<CustomerFacilityGetModel>>> GetCustomerFacilitiesByCustomerLocationIdAsync(AppState state, int customerLocationId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var customerLocationExist = await _context.CustomerLocations
                        .AnyAsync(x => x.CustomerLocationId == customerLocationId
                            && x.CustomerId == state.CustomerId);

                    if (!customerLocationExist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                IEnumerable<CustomerFacilityGetModel> model = await _context.CustomerFacilities
                    .AsNoTracking()
                    .Select(x => new CustomerFacilityGetModel
                    {
                        CustomerId = x.CustomerId,
                        CustomerFacilityId = x.CustomerFacilityId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        Name = x.Name,
                        Address = x.Address,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        PhoneNumber = x.PhoneNumber,
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

        public async Task<Result<IEnumerable<CustomerFacilityGetModel>>> GetCustomerLocationsSuperAdminAsync()
        {
            try
            {
                IEnumerable<CustomerFacilityGetModel> model = await _context.CustomerFacilities
                    .AsNoTracking()
                    .Select(x => new CustomerFacilityGetModel
                    {
                        CustomerId = x.CustomerId,
                        CustomerFacilityId = x.CustomerFacilityId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        Name = x.Name,
                        Address = x.Address,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        PhoneNumber = x.PhoneNumber,
                    })
                    .ToListAsync();

                if (model == null)
                {
                    return Result.Fail($"{nameof(CustomerFacility)} not found.");
                }
                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerFacilityGetModel>> GetCustomerFacilityAsync(AppState state, int customerFacilityId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var exist = await _context.CustomerFacilities
                        .Include(x => x.CustomerLocation)
                        .AnyAsync(x => x.CustomerFacilityId == customerFacilityId
                            && x.CustomerLocation.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerFacility)} not found.");
                    }
                }

                var model = await _context.CustomerFacilities
                    .AsNoTracking()
                    .Select(x => new CustomerFacilityGetModel
                    {
                        CustomerId = x.CustomerId,
                        CustomerFacilityId = x.CustomerFacilityId,
                        CustomerLocationId = x.CustomerLocationId.Value,
                        Name = x.Name,
                        Address = x.Address,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        PhoneNumber = x.PhoneNumber,
                    })
                    .SingleOrDefaultAsync(x => x.CustomerFacilityId == customerFacilityId);

                if (model == null)
                {
                    return Result.Fail($"{nameof(CustomerFacility)} not found.");
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