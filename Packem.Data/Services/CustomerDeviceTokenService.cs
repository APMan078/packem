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
    public class CustomerDeviceTokenService : ICustomerDeviceTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public CustomerDeviceTokenService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<CustomerDeviceTokenGetModel>> CreateCustomerDeviceTokenAsync(AppState state, CustomerDeviceTokenCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerDeviceId is null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceTokenCreateModel.CustomerDeviceId)} is required.");
                }

                var query = _context.CustomerDevices.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query
                        .Include(x => x.CustomerLocation)
                        .Where(x => x.CustomerDeviceId == model.CustomerDeviceId
                            && x.CustomerLocation.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query
                        .Where(x => x.CustomerDeviceId == model.CustomerDeviceId);
                }

                var customerDeviceExist = await query
                    .AnyAsync();

                if (!customerDeviceExist)
                {
                    return Result.Fail($"{nameof(CustomerDevice)} not found.");
                }

                if (!model.DeviceToken.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerDeviceTokenCreateModel.DeviceToken)} is required.");
                }
                else
                {
                    //var exist = await _context.CustomerDeviceTokens
                    //    .AnyAsync(x => x.DeviceToken.Trim().ToLower() == model.DeviceToken.Trim().ToLower()
                    //        && x.CustomerDeviceId == model.CustomerDeviceId);

                    var exist = await _context.CustomerDeviceTokens
                        .AnyAsync(x => x.DeviceToken.Trim().ToLower() == model.DeviceToken.Trim().ToLower());

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerDeviceTokenCreateModel.DeviceToken)} already exists.");
                    }

                    var device = await _context.CustomerDevices
                        .Include(x => x.CustomerDeviceTokens)
                        .SingleOrDefaultAsync(x => x.CustomerDeviceId == model.CustomerDeviceId);

                    foreach (var x in device.CustomerDeviceTokens)
                    {
                        // make sure there is only one active token per device
                        if (!x.IsValidated && x.IsActive)
                        {
                            return Result.Fail("Device has token that is not validated.");
                        }

                        // make sure there is only one active token per device
                        if (x.IsValidated && x.IsActive)
                        {
                            return Result.Fail("Device can already have one active token.");
                        }
                    }
                }

                var entity = new CustomerDeviceToken
                {
                    CustomerDeviceId = model.CustomerDeviceId,
                    DeviceToken = model.DeviceToken,
                    AddedDateTime = DateTime.Now,
                    IsValidated = false,
                    IsActive = true
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerDeviceTokenGetModel
                {
                    CustomerDeviceTokenId = entity.CustomerDeviceTokenId,
                    CustomerDeviceId = entity.CustomerDeviceId.Value,
                    DeviceToken = entity.DeviceToken,
                    AddedDateTime = entity.AddedDateTime,
                    IsValidated = entity.IsValidated,
                    ValidatedDateTime = entity.ValidatedDateTime,
                    IsActive = entity.IsActive,
                    DeactivedDateTime = entity.DeactivedDateTime
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<CustomerDeviceTokenGetModel>>> GetCustomerDeviceTokensByCustomerDeviceIdAsync(AppState state, int customerDeviceId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.CustomerDevices.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query
                        .Include(x => x.CustomerLocation)
                        .Where(x => x.CustomerDeviceId == customerDeviceId
                            && x.CustomerLocation.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query
                        .Where(x => x.CustomerDeviceId == customerDeviceId);
                }

                var customerDeviceExist = await query
                    .AnyAsync();

                if (!customerDeviceExist)
                {
                    return Result.Fail($"{nameof(CustomerDevice)} not found.");
                }

                var query2 = _context.CustomerDeviceTokens.AsQueryable();
                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query2 = query2
                        .Include(x => x.CustomerDevice)
                            .ThenInclude(x => x.CustomerLocation)
                        .Where(x => x.CustomerDeviceId == customerDeviceId
                            && x.CustomerDevice.CustomerLocation.CustomerId == state.CustomerId);
                }
                else
                {
                    query2 = query2.Where(x => x.CustomerDeviceId == customerDeviceId);
                }

                    IEnumerable<CustomerDeviceTokenGetModel> model = await query2
                        .AsNoTracking()
                        .Select(x => new CustomerDeviceTokenGetModel
                        {
                            CustomerDeviceTokenId = x.CustomerDeviceTokenId,
                            CustomerDeviceId = x.CustomerDeviceId.Value,
                            DeviceToken = x.DeviceToken,
                            AddedDateTime = x.AddedDateTime,
                            IsValidated = x.IsValidated,
                            ValidatedDateTime = x.ValidatedDateTime,
                            IsActive = x.IsActive,
                            DeactivedDateTime = x.DeactivedDateTime
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

        public async Task<Result<CustomerDeviceTokenGetModel>> GetCustomerDeviceTokenAsync(AppState state, int customerDeviceTokenId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    var exist = await _context.CustomerDeviceTokens
                        .Include(x => x.CustomerDevice)
                            .ThenInclude(x => x.CustomerLocation)
                        .AnyAsync(x => x.CustomerDeviceTokenId == customerDeviceTokenId
                            && x.CustomerDevice.CustomerLocation.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerDeviceToken)} not found.");
                    }
                }

                var model = await _context.CustomerDeviceTokens
                    .AsNoTracking()
                    .Select(x => new CustomerDeviceTokenGetModel
                    {
                        CustomerDeviceTokenId = x.CustomerDeviceTokenId,
                        CustomerDeviceId = x.CustomerDeviceId.Value,
                        DeviceToken = x.DeviceToken,
                        AddedDateTime = x.AddedDateTime,
                        IsValidated = x.IsValidated,
                        ValidatedDateTime = x.ValidatedDateTime,
                        IsActive = x.IsActive,
                        DeactivedDateTime = x.DeactivedDateTime
                    })
                    .SingleOrDefaultAsync(x => x.CustomerDeviceTokenId == customerDeviceTokenId);

                if (model == null)
                {
                    return Result.Fail($"{nameof(CustomerDeviceToken)} not found.");
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
