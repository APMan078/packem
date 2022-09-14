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
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public CustomerService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<CustomerGetModel>> CreateCustomerAsync(CustomerCreateModel model)
        {
            try
            {
                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Customers
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower());

                    if (exist)
                    {
                        return Result.Fail($"{nameof(Customer)} already exists.");
                    }
                }

                if (!model.Address.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.Address)} is required.");
                }

                if (!model.City.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.City)} is required.");
                }

                if (!model.StateProvince.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.StateProvince)} is required.");
                }

                if (!model.ZipPostalCode.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.ZipPostalCode)} is required.");
                }

                if (!model.PhoneNumber.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.PhoneNumber)} is required.");
                }

                if (!model.PointOfContact.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.PointOfContact)} is required.");
                }

                if (!model.ContactEmail.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerCreateModel.ContactEmail)} is required.");
                }

                var entity = new Customer
                {
                    Name = model.Name,
                    Address = model.Address,
                    Address2 = model.Address2,
                    City = model.City,
                    StateProvince = model.StateProvince,
                    ZipPostalCode = model.ZipPostalCode,
                    PhoneNumber = model.PhoneNumber,
                    PointOfContact = model.PointOfContact,
                    ContactEmail = model.ContactEmail,
                    IsActive = true
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();


                int[] defaultUomIds = new int[] { 1, 5, 8, 11, 20, 21, 25, 26, 27 };

                List<UnitOfMeasureCustomer> unitOfMeasureCustomers = new List<UnitOfMeasureCustomer>();

                foreach(int id in defaultUomIds)
                {
                    UnitOfMeasureCustomer unitOfMeasureCustomer = new UnitOfMeasureCustomer
                    {
                        CustomerId = entity.CustomerId,
                        UnitOfMeasureId = id,
                    };

                    unitOfMeasureCustomers.Add(unitOfMeasureCustomer);
                }

                await _context.AddRangeAsync(unitOfMeasureCustomers);
                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerGetModel
                {
                    CustomerId = entity.CustomerId,
                    Name = entity.Name,
                    Address = entity.Address,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    PhoneNumber = entity.PhoneNumber,
                    PointOfContact = entity.PointOfContact,
                    ContactEmail = entity.ContactEmail,
                    IsActive = true
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerGetModel>> EditCustomerAsync(CustomerEditModel model)
        {
            try
            {
                var entity = await _context.Customers
                    .SingleOrDefaultAsync(x => x.CustomerId == model.CustomerId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.Customers
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.Name.Trim().ToLower() != entity.Name.Trim().ToLower());

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerEditModel.Name)} is already exist.");
                    }
                }

                if (!model.Address.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.Address)} is required.");
                }

                if (!model.City.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.City)} is required.");
                }

                if (!model.StateProvince.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.StateProvince)} is required.");
                }

                if (!model.ZipPostalCode.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.ZipPostalCode)} is required.");
                }

                if (!model.PhoneNumber.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.PhoneNumber)} is required.");
                }

                if (!model.PointOfContact.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.PointOfContact)} is required.");
                }

                if (!model.ContactEmail.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerEditModel.ContactEmail)} is required.");
                }

                entity.Name = model.Name;
                entity.Address = model.Address;
                entity.Address2 = model.Address2;
                entity.City = model.City;
                entity.StateProvince = model.StateProvince;
                entity.ZipPostalCode = model.ZipPostalCode;
                entity.PhoneNumber = model.PhoneNumber;
                entity.PointOfContact = model.PointOfContact;
                entity.ContactEmail = model.ContactEmail;

                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerGetModel
                {
                    CustomerId = entity.CustomerId,
                    Name = entity.Name,
                    Address = entity.Address,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    PhoneNumber = entity.PhoneNumber,
                    PointOfContact = entity.PointOfContact,
                    ContactEmail = entity.ContactEmail,
                    IsActive = entity.IsActive
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerGetModel>> EditCustomerIsActiveAsync(AppState state, CustomerIsActiveEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(CustomerIsActiveEditModel.CustomerId)} is required.");
                }

                if (model.IsActive is null)
                {
                    return Result.Fail($"{nameof(CustomerIsActiveEditModel.IsActive)} is required.");
                }

                var entity = await _context.Customers
                    .SingleOrDefaultAsync(x => x.CustomerId == model.CustomerId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                entity.IsActive = model.IsActive.Value;
                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerGetModel
                {
                    CustomerId = entity.CustomerId,
                    Name = entity.Name,
                    Address = entity.Address,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    PhoneNumber = entity.PhoneNumber,
                    PointOfContact = entity.PointOfContact,
                    ContactEmail = entity.ContactEmail,
                    IsActive = entity.IsActive
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<CustomerGetModel>>> GetCustomersAsync()
        {
            try
            {
                IEnumerable<CustomerGetModel> model = await _context.Customers
                    .AsNoTracking()
                    .Select(x => new CustomerGetModel
                    {
                        CustomerId = x.CustomerId,
                        Name = x.Name,
                        Address = x.Address,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        PhoneNumber = x.PhoneNumber,
                        PointOfContact = x.PointOfContact,
                        ContactEmail = x.ContactEmail,
                        IsActive = x.IsActive
                    })
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<int>> GetCustomerDefaultThresholdAsync(int customerId)
        {
            try
            {
                int defaultThreshold = 0;
                int? t = await _context.Customers
                    .Where(x => x.CustomerId == customerId)
                    .Select(x => x.DefaultItemThreshold)
                    .SingleOrDefaultAsync();
                if (t != null)
                {
                    defaultThreshold = (int)t;
                }

                
                return Result.Ok(defaultThreshold);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }

        }
        public async Task<Result<CustomerGetModel>> UpdateCustomerDefaultThresholdAsync(int customerId, int? threshold)
        {
            try
            {
              
                var entity = await _context.Customers
                    .SingleOrDefaultAsync(x => x.CustomerId == customerId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                if (threshold is null)
                {
                    return Result.Fail($"Threshold is required.");
                }

                entity.DefaultItemThreshold = threshold;
                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerGetModel
                {
                    CustomerId = entity.CustomerId,
                    Name = entity.Name,
                    Address = entity.Address,
                    Address2 = entity.Address2,
                    City = entity.City,
                    StateProvince = entity.StateProvince,
                    ZipPostalCode = entity.ZipPostalCode,
                    PhoneNumber = entity.PhoneNumber,
                    PointOfContact = entity.PointOfContact,
                    ContactEmail = entity.ContactEmail,
                    IsActive = entity.IsActive,
                    DefaultThreshold = entity.DefaultItemThreshold.Value,
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerGetModel>> GetCustomerAsync(int customerId)
        {
            try
            {
                var model = await _context.Customers
                    .Where(x => x.CustomerId == customerId)
                    .AsNoTracking()
                    .Select(x => new CustomerGetModel
                    {
                        CustomerId = x.CustomerId,
                        Name = x.Name,
                        Address = x.Address,
                        Address2 = x.Address2,
                        City = x.City,
                        StateProvince = x.StateProvince,
                        ZipPostalCode = x.ZipPostalCode,
                        PhoneNumber = x.PhoneNumber,
                        PointOfContact = x.PointOfContact,
                        ContactEmail = x.ContactEmail,
                        IsActive = x.IsActive
                    })
                    .SingleOrDefaultAsync();

                if (model == null)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerGetCurrentModel>> GetCurrentCustomerAsync(AppState state)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Customers.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var customerLocation = await _context.CustomerLocations
                        .AsNoTracking()
                        .Select(x => new { x.CustomerId, x.CustomerLocationId })
                        .SingleOrDefaultAsync(x => x.CustomerLocationId == state.CustomerLocationId);

                    if (customerLocation is null)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }

                    query = query.Where(x => x.CustomerId == customerLocation.CustomerId);
                }

                var model = await query
                    .AsNoTracking()
                    .Include(x => x.CustomerLocations)
                        .ThenInclude(x => x.CustomerFacilities)
                    .Select(x => new CustomerGetCurrentModel.Customer
                    {
                        CustomerId = x.CustomerId,
                        Name = x.Name,
                        CustomerLocations = x.CustomerLocations
                            .Select(z => new CustomerGetCurrentModel.CustomerLocation
                            {
                                CustomerLocationId = z.CustomerLocationId,
                                Name = z.Name,
                                //State = (StateEnum)Convert.ToInt32(z.StateId),
                                CustomerFacilities = z.CustomerFacilities
                                    .Select(a => new CustomerGetCurrentModel.CustomerFacility
                                    {
                                        CustomerFacilityId = a.CustomerFacilityId,
                                        Name = a.Name
                                    })
                            })
                    })
                    .ToListAsync();

                if (model == null)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                var m = new CustomerGetCurrentModel();
                m.Customers = model;

                return Result.Ok(m);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerGetCurrentMobileModel>> GetCurrentCustomerForDeviceAsync(CustomerDeviceTokenAuthModel state)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var model = await _context.Customers
                    .AsNoTracking()
                    .Include(x => x.CustomerLocations)
                        .ThenInclude(x => x.CustomerFacilities)
                    .Select(x => new CustomerGetCurrentMobileModel
                    {
                        CustomerId = x.CustomerId,
                        Name = x.Name,
                        CustomerLocation = x.CustomerLocations
                            .Select(z => new CustomerGetCurrentMobileModel.CurrentCustomerLocation
                            {
                                CustomerLocationId = z.CustomerLocationId,
                                Name = z.Name,
                                //State = z.StateId,
                                CustomerFacilities = z.CustomerFacilities
                                    .Select(c => new CustomerGetCurrentMobileModel.CustomerFacility
                                    {
                                        CustomerFacilityId = c.CustomerFacilityId,
                                        Name = c.Name
                                    })
                            })
                            .SingleOrDefault(x => x.CustomerLocationId == state.CustomerLocationId)
                    })
                    .SingleOrDefaultAsync(x => x.CustomerId == state.CustomerId
                        && x.CustomerLocation.CustomerLocationId == state.CustomerLocationId);

                if (model == null)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
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