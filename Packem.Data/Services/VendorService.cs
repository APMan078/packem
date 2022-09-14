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
    public class VendorService : IVendorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public VendorService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<VendorGetModel>> CreateVendorAsync(AppState state, VendorCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(VendorCreateModel.CustomerId)} is required.");
                }

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

                var customerExist = await query
                    .AnyAsync();

                if (!customerExist)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                if (!model.Account.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.Account)} is required.");
                }
                else
                {
                    var exist = await _context.Vendors
                        .AnyAsync(x => x.VendorNo.Trim().ToLower() == model.Account.Trim().ToLower()
                            && x.CustomerId == model.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(VendorCreateModel.Account)} is already exist.");
                    }
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Vendors
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.CustomerId == model.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(VendorCreateModel.Name)} is already exist.");
                    }
                }

                if (!model.Contact.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.Contact)} is required.");
                }

                if (!model.City.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.City)} is required.");
                }

                if (!model.StateProvince.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.StateProvince)} is required.");
                }

                if (!model.ZipPostalCode.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.ZipPostalCode)} is required.");
                }

                if (!model.Phone.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.Phone)} is required.");
                }

                var entity = new Vendor
                {
                    CustomerId = model.CustomerId,
                    VendorNo = model.Account,
                    Name = model.Name,
                    PointOfContact = model.Contact,
                    PhoneNumber = model.Phone,
                    Address1 = model.Address,
                    Address2 = model.Address2,
                    City = model.City,
                    StateProvince = model.StateProvince,
                    ZipPostalCode = model.ZipPostalCode,
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new VendorGetModel
                {
                    VendorId = entity.VendorId,
                    CustomerId = entity.CustomerId.Value,
                    VendorNo = entity.VendorNo,
                    Name = entity.Name,
                    Contact = entity.PointOfContact,
                    Address = entity.Address1,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    Phone = entity.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<VendorGetModel>> EditVendorAsync(AppState state, VendorEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.VendorId is null)
                {
                    return Result.Fail($"{nameof(VendorEditModel.VendorId)} is required.");
                }

                Vendor entity;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    entity = await _context.Vendors
                        .SingleOrDefaultAsync(x => x.VendorId == model.VendorId
                            && x.CustomerId == state.CustomerId);
                }
                else
                {
                    entity = await _context.Vendors
                        .SingleOrDefaultAsync(x => x.VendorId == model.VendorId);
                }

                if (entity is null)
                {
                    return Result.Fail($"{nameof(Vendor)} not found.");
                }

                if (!model.Account.HasValue())
                {
                    return Result.Fail($"{nameof(VendorEditModel.Account)} is required.");
                }
                else
                {
                    var exist = await _context.Vendors
                        .AnyAsync(x => x.VendorNo.Trim().ToLower() == model.Account.Trim().ToLower()
                            && x.VendorNo.Trim().ToLower() != entity.VendorNo.Trim().ToLower()
                            && x.CustomerId == entity.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(VendorEditModel.Account)} is already exist.");
                    }
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(VendorEditModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Vendors
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.Name.Trim().ToLower() != entity.Name.Trim().ToLower()
                            && x.CustomerId == entity.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(VendorEditModel.Name)} is already exist.");
                    }
                }

                if (!model.Contact.HasValue())
                {
                    return Result.Fail($"{nameof(VendorEditModel.Contact)} is required.");
                }
                
                
                if (!model.Address.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.Address)} is required.");
                }
                
                if (!model.City.HasValue())
                {
                    return Result.Fail($"{nameof(VendorCreateModel.City)} is required.");
                }

                if (!model.Phone.HasValue())
                {
                    return Result.Fail($"{nameof(VendorEditModel.Phone)} is required.");
                }

                entity.VendorNo = model.Account;
                entity.Name = model.Name;
                entity.PointOfContact = model.Contact;
                entity.Address1 = model.Address;
                entity.PhoneNumber = model.Phone;
                entity.ZipPostalCode = model.ZipPostalCode;
                entity.StateProvince = model.StateProvince;
                entity.City = model.City;
                entity.Address2 = model.Address2;
                await _context.SaveChangesAsync();

                return Result.Ok(new VendorGetModel
                {
                    VendorId = entity.VendorId,
                    CustomerId = entity.CustomerId.Value,
                    VendorNo = entity.VendorNo,
                    Name = entity.Name,
                    Contact = entity.PointOfContact,
                    Address = entity.Address1,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    Phone = entity.PointOfContact
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<VendorItemGetModel>>> GetVendorItemsAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Vendors
                    .Include(x => x.ItemVendors)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.Inventory)
                                .ThenInclude(x => x.InventoryBins)
                    .AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                IEnumerable<VendorItemGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new VendorItemGetModel
                    {
                        VendorId = x.VendorId,
                        Account = x.VendorNo,
                        Items = x.ItemVendors.Count(),
                        Name = x.Name,
                        Contact = x.PointOfContact,
                        Address = x.Address1,
                        Phone = x.PhoneNumber
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Vendor)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<VendorLookupGetModel>>> GetVendorLookupAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Vendors
                    .AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == customerId
                        && (x.Name.Trim().ToLower().Contains(searchText)
                            || x.PointOfContact.Trim().ToLower().Contains(searchText)
                            || x.Address1.Trim().ToLower().Contains(searchText)
                            || x.PhoneNumber.Trim().ToLower().Contains(searchText)));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }

                IEnumerable<VendorLookupGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new VendorLookupGetModel
                    {
                        VendorId = x.VendorId,
                        Name = x.Name,
                        Contact = x.PointOfContact,
                        Address = x.Address1,
                        Phone = x.PhoneNumber
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Vendor)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<VendorLookupNameGetModel>>> GetVendorLookupByNameAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Vendors
                    .AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == customerId
                        && x.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }

                IEnumerable<VendorLookupNameGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new VendorLookupNameGetModel
                    {
                        VendorId = x.VendorId,
                        Name = x.Name
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Vendor)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<VendorLookupNameGetModel>>> GetVendorLookupByNameDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.ItemVendors
                    .Include(x => x.Vendor)
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == state.CustomerId
                        && x.ItemId == itemId
                        && x.Vendor.Name.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == state.CustomerId
                        && x.ItemId == itemId);
                }

                IEnumerable<VendorLookupNameGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new VendorLookupNameGetModel
                    {
                        VendorId = x.Vendor.VendorId,
                        Name = x.Vendor.Name
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(Vendor)} not found.");
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