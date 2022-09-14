using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.Helpers;
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
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public UserService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<UserGetModel>> CreateUserAsync(AppState state, UserCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(UserCreateModel)} is required.");
                }

                if (model.Role is null)
                {
                    return Result.Fail($"{nameof(UserCreateModel.Role)} is required.");
                }

                if (model.Role != RoleEnum.SuperAdmin)
                {
                    if (model.CustomerId is null)
                    {
                        return Result.Fail($"{nameof(UserCreateModel.CustomerId)} is required.");
                    }

                    var exist = await _context.Customers
                        .AnyAsync(x => x.CustomerId == model.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }
                else
                {
                    // if SuperAdmin and CustomerId has value, make it null
                    model.CustomerId = null;
                }

                // if SuperAdmin or Admin and CustomerLocationId has value, make it null
                if (model.Role == RoleEnum.SuperAdmin
                    || model.Role == RoleEnum.Admin)
                {
                    model.CustomerLocationId = null;
                }

                if (model.Role == RoleEnum.OpsManager
                    || model.Role == RoleEnum.Operator
                    || model.Role == RoleEnum.Viewer)
                {
                    if (model.CustomerLocationId is null)
                    {
                        return Result.Fail($"{nameof(UserCreateModel.CustomerLocationId)} is required.");
                    }

                    var query = _context.CustomerLocations.AsQueryable();

                    if (state.Role == RoleEnum.Admin)
                    {
                        query = query.Where(x => x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == state.CustomerId);
                    }
                    else if (state.Role == RoleEnum.SuperAdmin)
                    {
                        query = query.Where(x => x.CustomerLocationId == model.CustomerLocationId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(UserCreateModel.Name)} is required.");
                }

                if (!model.Username.HasValue())
                {
                    return Result.Fail($"{nameof(UserCreateModel.Username)} is required.");
                }
                else
                {
                    var exist = await _context.Users
                        .AnyAsync(x => x.Username.Trim().ToLower() == model.Username.Trim().ToLower());

                    if (exist)
                    {
                        return Result.Fail($"{nameof(UserCreateModel.Username)} already exists.");
                    }
                }

                if (!model.Email.HasValue())
                {
                    return Result.Fail($"{nameof(UserCreateModel.Email)} is required.");
                }
                else
                {
                    if (!model.Email.IsValidEmail())
                    {
                        return Result.Fail($"{nameof(UserCreateModel.Email)} is invalid.");
                    }
                    else
                    {
                        var exist = await _context.Users
                            .AnyAsync(x => x.Email.Trim().ToLower() == model.Email.Trim().ToLower());

                        if (exist)
                        {
                            return Result.Fail($"{nameof(UserCreateModel.Email)} already exists.");
                        }
                    }
                }

                if (state.Role == RoleEnum.Admin)
                {
                    if (model.Role == RoleEnum.SuperAdmin)
                    {
                        return Result.Fail($"You cannot create a {nameof(RoleEnum.SuperAdmin)} role. Please choose other role.");
                    }
                }

                if (model.Role == RoleEnum.Viewer)
                {
                    if (model.VendorIds.Count() == 0)
                    {
                        return Result.Fail($"{nameof(UserCreateModel.VendorIds)} is required.");
                    }
                }

                if (!model.Password.HasValue())
                {
                    return Result.Fail($"{nameof(UserCreateModel.Password)} is required.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var pass = CryptographicHelper.EncryptPassword(model.Password);

                        var entity = new User
                        {
                            CustomerId = model.CustomerId,
                            CustomerLocationId = model.CustomerLocationId,
                            Name = model.Name,
                            Username = model.Username,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            RoleId = model.Role.ToInt(),
                            Password = pass.Hash,
                            PasswordSalt = pass.Salt,
                            IsActive = true
                        };

                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        if (model.Role == RoleEnum.Viewer)
                        {
                            foreach (var x in model.VendorIds)
                            {
                                var vendor = await _context.Vendors
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(z => z.VendorId == x
                                        && z.CustomerId == model.CustomerId);

                                if (vendor is null)
                                {
                                    return Result.Fail($"{nameof(Vendor)} not found.");
                                }
                                else
                                {
                                    var userRoleVendor = new UserRoleVendor
                                    {
                                        UserId = entity.UserId,
                                        RoleId = entity.RoleId,
                                        VendorId = x
                                    };
                                    _context.Add(userRoleVendor);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                        await transaction.CommitAsync();

                        return Result.Ok(new UserGetModel
                        {
                            UserId = entity.UserId,
                            CustomerId = entity.CustomerId,
                            CustomerLocationId = entity.CustomerLocationId,
                            Name = entity.Name,
                            Username = entity.Username,
                            Email = entity.Email,
                            PhoneNumber = entity.PhoneNumber,
                            RoleId = entity.RoleId,
                            IsActive = entity.IsActive
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

        public async Task<Result<UserGetModel>> EditUserAsync(AppState state, UserEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(UserEditModel)} is required.");
                }

                if (model.Role is null)
                {
                    return Result.Fail($"{nameof(UserEditModel.Role)} is required.");
                }

                if (model.Role == RoleEnum.OpsManager
                    || model.Role == RoleEnum.Operator
                    || model.Role == RoleEnum.Viewer)
                {
                    if (model.CustomerLocationId is null)
                    {
                        return Result.Fail($"{nameof(UserCreateModel.CustomerLocationId)} is required.");
                    }

                    var query = _context.CustomerLocations.AsQueryable();

                    if (state.Role == RoleEnum.Admin)
                    {
                        query = query.Where(x => x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == state.CustomerId);
                    }
                    else if (state.Role == RoleEnum.SuperAdmin)
                    {
                        query = query.Where(x => x.CustomerLocationId == model.CustomerLocationId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(CustomerLocation)} not found.");
                    }
                }

                if (state.Role == RoleEnum.Admin)
                {
                    var superAdminExist = await _context.Users
                        .AnyAsync(x => x.UserId == model.UserId
                            && x.RoleId == RoleEnum.SuperAdmin.ToInt());

                    if (superAdminExist)
                    {
                        return Result.Fail($"You cannot edit {nameof(RoleEnum.SuperAdmin)} user.");
                    }
                }

                // if SuperAdmin or Admin and CustomerLocationId has value, make it null
                if (model.Role == RoleEnum.SuperAdmin
                    || model.Role == RoleEnum.Admin)
                {
                    model.CustomerLocationId = null;
                }

                var user = await _context.Users
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.UserId == model.UserId);

                if (user is null)
                {
                    return Result.Fail($"{nameof(User)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(UserCreateModel.Name)} is required.");
                }

                if (!model.Username.HasValue())
                {
                    return Result.Fail($"{nameof(UserCreateModel.Username)} is required.");
                }
                else
                {
                    var exist = await _context.Users
                        .AnyAsync(x => x.Username.Trim().ToLower() == model.Username.Trim().ToLower()
                            && x.Username.Trim().ToLower() != user.Username.Trim().ToLower());

                    if (exist)
                    {
                        return Result.Fail($"{nameof(UserCreateModel.Username)} is already exist.");
                    }
                }

                if (!model.Email.HasValue())
                {
                    return Result.Fail($"{nameof(UserCreateModel.Email)} is required.");
                }
                else
                {
                    if (!model.Email.IsValidEmail())
                    {
                        return Result.Fail($"{nameof(UserCreateModel.Email)} is invalid.");
                    }
                    else
                    {
                        var exist = await _context.Users
                            .AnyAsync(x => x.Email.Trim().ToLower() == model.Email.Trim().ToLower()
                                && x.Email.Trim().ToLower() != user.Email.Trim().ToLower());

                        if (exist)
                        {
                            return Result.Fail($"{nameof(UserCreateModel.Email)} is already exist.");
                        }
                    }
                }

                if (state.Role == RoleEnum.Admin)
                {
                    if (model.Role == RoleEnum.SuperAdmin)
                    {
                        return Result.Fail($"You cannot create a {nameof(RoleEnum.SuperAdmin)} role. Please choose other role.");
                    }
                }

                if (model.Role == RoleEnum.Viewer)
                {
                    if (model.VendorIds.Count() == 0)
                    {
                        return Result.Fail($"{nameof(UserCreateModel.VendorIds)} is required.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.Users
                            .SingleOrDefaultAsync(x => x.UserId == model.UserId);

                        entity.CustomerLocationId = model.CustomerLocationId;
                        entity.Name = model.Name;
                        entity.Username = model.Username;
                        entity.Email = model.Email;
                        entity.PhoneNumber = model.PhoneNumber;
                        entity.RoleId = model.Role.ToInt();
                        entity.IsActive = model.IsActive;
                        await _context.SaveChangesAsync();

                        if (model.Role == RoleEnum.Viewer)
                        {
                            var userRoleVendors = await _context.UserRoleVendors
                                .AsNoTracking()
                                .Where(x => x.UserId == entity.UserId
                                    && x.RoleId == RoleEnum.Viewer.ToInt())
                                .ToListAsync();

                            foreach (var x in model.VendorIds)
                            {
                                var vendor = await _context.Vendors
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(z => z.VendorId == x
                                        && z.CustomerId == entity.CustomerId);

                                if (vendor is null)
                                {
                                    return Result.Fail($"{nameof(Vendor)} not found.");
                                }
                                else
                                {
                                    if (!userRoleVendors.Any(z => z.VendorId == x)) // not exist, create
                                    {
                                        var userRoleVendor = new UserRoleVendor
                                        {
                                            UserId = entity.UserId,
                                            RoleId = entity.RoleId,
                                            VendorId = x
                                        };
                                        _context.Add(userRoleVendor);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            // remove user's vendor in db which is not exist in model.VendorIds
                            var toRemove = userRoleVendors.Select(x => x.VendorId.Value).Except(model.VendorIds).ToList();
                            foreach (var x in toRemove)
                            {
                                var userRoleVendor = await _context.UserRoleVendors
                                    .SingleOrDefaultAsync(z => z.UserId == entity.UserId
                                        && z.RoleId == RoleEnum.Viewer.ToInt()
                                        && z.VendorId == x);

                                userRoleVendor.Deleted = true;
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            var userRoleVendors = await _context.UserRoleVendors
                                .Where(x => x.UserId == entity.UserId
                                    && x.RoleId == RoleEnum.Viewer.ToInt())
                                .ToListAsync();

                            foreach (var x in userRoleVendors)
                            {
                                x.Deleted = true;
                            }

                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();

                        return Result.Ok(new UserGetModel
                        {
                            UserId = entity.UserId,
                            CustomerId = entity.CustomerId,
                            CustomerLocationId = entity.CustomerLocationId,
                            Name = entity.Name,
                            Username = entity.Username,
                            Email = entity.Email,
                            PhoneNumber = entity.PhoneNumber,
                            RoleId = entity.RoleId,
                            IsActive = entity.IsActive
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

        public async Task<Result<UserGetModel>> EditUserIsActiveAsync(AppState state, UserIsActiveEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.UserId is null)
                {
                    return Result.Fail($"{nameof(UserIsActiveEditModel.UserId)} is required.");
                }

                if (model.IsActive is null)
                {
                    return Result.Fail($"{nameof(UserIsActiveEditModel.IsActive)} is required.");
                }

                var entity = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserId == model.UserId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(User)} not found.");
                }

                entity.IsActive = model.IsActive.Value;
                await _context.SaveChangesAsync();

                return Result.Ok(new UserGetModel
                {
                    UserId = entity.UserId,
                    CustomerId = entity.CustomerId,
                    CustomerLocationId = entity.CustomerLocationId,
                    Name = entity.Name,
                    Username = entity.Username,
                    Email = entity.Email,
                    PhoneNumber = entity.PhoneNumber,
                    RoleId = entity.RoleId,
                    IsActive = entity.IsActive
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<UserGetModel>> EditUserPasswordAsync(AppState state, UserPasswordEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.UserId is null)
                {
                    return Result.Fail($"{nameof(UserPasswordEditModel.UserId)} is required.");
                }

                if (!model.OldPassword.HasValue())
                {
                    return Result.Fail($"{nameof(UserPasswordEditModel.OldPassword)} is required.");
                }

                if (!model.NewPassword.HasValue())
                {
                    return Result.Fail($"{nameof(UserPasswordEditModel.NewPassword)} is required.");
                }

                var entity = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserId == model.UserId);

                if (entity is null)
                {
                    return Result.Fail($"{nameof(User)} not found.");
                }

                var pass = CryptographicHelper.VerifyPassword(model.OldPassword, entity.PasswordSalt, entity.Password);

                if (!pass)
                {
                    return Result.Fail($"{nameof(model.OldPassword)} is invalid.");
                }

                var newPass = CryptographicHelper.EncryptPassword(model.NewPassword);

                entity.Password = newPass.Hash;
                entity.PasswordSalt = newPass.Salt;
                await _context.SaveChangesAsync();

                return Result.Ok(new UserGetModel
                {
                    UserId = entity.UserId,
                    CustomerId = entity.CustomerId,
                    CustomerLocationId = entity.CustomerLocationId,
                    Name = entity.Name,
                    Username = entity.Username,
                    Email = entity.Email,
                    PhoneNumber = entity.PhoneNumber,
                    RoleId = entity.RoleId,
                    IsActive = entity.IsActive
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<UserGetModel>>> GetUsersAsync(AppState state)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Users.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId);
                }

                IEnumerable<UserGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new UserGetModel
                    {
                        UserId = x.UserId,
                        CustomerId = x.CustomerId,
                        CustomerLocationId = x.CustomerLocationId,
                        Name = x.Name,
                        Username = x.Username,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        RoleId = x.RoleId,
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

        public async Task<Result<List<UserGetModel>>> GetUsersByCustomerIdAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Users.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query.Where(x => x.CustomerId == state.CustomerId);
                }

                var model = await query
                    .AsNoTracking()
                    .Select(x => new UserGetModel
                    {
                        UserId = x.UserId,
                        CustomerId = x.CustomerId,
                        CustomerLocationId = x.CustomerLocationId,
                        Name = x.Name,
                        Username = x.Username,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        RoleId = x.RoleId,
                        IsActive = x.IsActive
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

        public async Task<Result<UserGetModel>> GetUserAsync(AppState state, int userId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Users.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query.Where(x => x.CustomerLocationId == state.CustomerLocationId);
                }

                var model = await query
                    .AsNoTracking()
                    .Select(x => new UserGetModel
                    {
                        UserId = x.UserId,
                        CustomerId = x.CustomerId,
                        CustomerLocationId = x.CustomerLocationId,
                        Name = x.Name,
                        Username = x.Username,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        RoleId = x.RoleId,
                        IsActive = x.IsActive
                    })
                    .SingleOrDefaultAsync(x => x.UserId == userId);

                if (model is null)
                {
                    return Result.Fail($"{nameof(User)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<UserVendorGetModel>>> GetUserVendorAsync(AppState state, int userId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                IEnumerable<UserVendorGetModel> model = await _context.UserRoleVendors
                    .Include(x => x.Vendor)
                    .AsNoTracking()
                    .Where(x => x.UserId == userId
                        && x.RoleId == RoleEnum.Viewer.ToInt())
                    .Select(x => new UserVendorGetModel
                    {
                        VendorId = x.Vendor.VendorId,
                        VendorNo = x.Vendor.VendorNo,
                        Name = x.Vendor.Name
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
    }
}