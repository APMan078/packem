using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Packem.Data.Helpers;
using Packem.Data.Interfaces;
using Packem.Data.Models;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;
        private readonly IEmailService _emailSvc;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context,
            IExceptionService exceptionService,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _exceptionService = exceptionService;
            _emailSvc = emailService;
            _configuration = configuration;
        }

        private async Task<Result<UserTokenModel>> BuildTokenAsync(UserLoginModel model)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.Customer)
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    x.Email,
                    x.Name,
                    x.Password,
                    x.PasswordSalt,
                    x.RoleId,
                    x.CustomerId,
                    CustomerAddress1 = x.Customer.Address,
                    CustomerAddress2 = x.Customer.Address2,
                    CustomerStateOrProvince = x.Customer.StateProvince,
                    CustomerZip = x.Customer.ZipPostalCode,
                    CustomerCity = x.Customer.City,
                    CustomerPhoneNumber = x.Customer.PhoneNumber,
                    CustomerName = x.Customer.Name,
                    x.CustomerLocationId
                })
                .SingleOrDefaultAsync(x => x.Username.Trim().ToLower() == model.Username.Trim().ToLower()
                    || x.Email.Trim().ToLower() == model.Username.Trim().ToLower());

            if (user == null)
            {
                return Result.Fail($"Invalid {nameof(model.Username)} or {nameof(model.Password)}.");
            }

            var pass = CryptographicHelper.VerifyPassword(model.Password, user.PasswordSalt, user.Password);

            if (!pass)
            {
                return Result.Fail($"Invalid {nameof(model.Username)} or {nameof(model.Password)}.");
            }

            var claims = new List<Claim>()
            {
                new Claim(CustomClaim.Claim_UserId, user.UserId.ToString()),
                new Claim(CustomClaim.Claim_Username, user.Username),
                new Claim(CustomClaim.Claim_Fullname, user.Name),
                new Claim(CustomClaim.Claim_Email, user.Email),
            };

            if (user.RoleId is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.RoleId.ToString()));
                claims.Add(new Claim(CustomClaim.Claim_UserRole, user.RoleId.ToString()));
            }

            if (user.CustomerId is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerId, user.CustomerId.ToString()));
            }

            if (user.CustomerLocationId is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerLocationId, user.CustomerLocationId.ToString()));
            }

            if (user.CustomerName is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerName, user.CustomerName));
            }

            if (user.CustomerAddress1 is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerAddress1, user.CustomerAddress1));
            }

            if (user.CustomerAddress2 is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerAddress2, user.CustomerAddress2));
            }

            if (user.CustomerCity is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerCity, user.CustomerCity));
            }

            if (user.CustomerStateOrProvince is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerStateOrProvince, user.CustomerStateOrProvince));
            }

            if (user.CustomerZip is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerZip, user.CustomerZip));
            }

            if (user.CustomerPhoneNumber is not null)
            {
                claims.Add(new Claim(CustomClaim.Claim_CustomerPhoneNumber, user.CustomerPhoneNumber));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiration, signingCredentials: creds);

            return Result.Ok(new UserTokenModel()
            {
                UserId = user.UserId,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            });
        }

        private async Task<Result<UserTemporaryTokenModel>> CreateTemporaryResetPasswordToken(ResetPasswordModel model)
        {
            try
            {
                SmtpHelper smtp = _configuration.GetSection("smtpDetails").Get<SmtpHelper>();

                if (!model.EmailAddress.HasValue())
                {
                    return Result.Fail("Please enter a valid email address.");
                }

                var user = await _context.Users
                .AsNoTracking()
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    x.Email,
                    x.Name,
                    x.Password,
                    x.PasswordSalt,
                    x.RoleId,
                    x.CustomerId,
                    CustomerName = x.Customer.Name,
                    x.CustomerLocationId
                })
                .SingleOrDefaultAsync(x => x.Email == model.EmailAddress.Trim().ToLower());

                if (user == null)
                {
                    return Result.Fail($"User not found. Please try again or contact support.");
                }

                var claims = new List<Claim>()
                {
                    new Claim(CustomClaim.Claim_UserId, user.UserId.ToString()),
                    new Claim(CustomClaim.Claim_Username, user.Username),
                    new Claim(CustomClaim.Claim_Fullname, user.Name),
                    new Claim(CustomClaim.Claim_Email, user.Email),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiration = DateTime.UtcNow.AddHours(1);
                var tokenScaffolding = new JwtSecurityToken(issuer: null, audience: null,
                    claims: claims, expires: expiration, signingCredentials: creds);

                var token = new JwtSecurityTokenHandler().WriteToken(tokenScaffolding);

                EmailNameAddress ToAddr = new EmailNameAddress()
                {
                    EmailId = model.EmailAddress.Trim(),
                    Name = user.Name
                };

                string url = new StringBuilder($"{_configuration.GetValue<string>("webUrl")}/password-reset?token={token}").ToString();
                string subject = "Packem Password Reset";
                AlternateView body = new EmailDispatch(user.Name.Trim(), url).PasswordResetTemplate;
                _emailSvc.SendEmail(body, subject, new EmailNameAddress[] { ToAddr }, smtp);

                return Result.Ok(new UserTemporaryTokenModel()
                {
                    Token = token,
                    Expiration = expiration
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<UserTokenModel>> AuthenticateUserAsync(UserLoginModel model)
        {
            try
            {
                return await BuildTokenAsync(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerDeviceTokenGetModel>> ValidateCustomerDeviceTokenAsync(CustomerDeviceTokenValidateTokenModel model)
        {
            try
            {
                //var customerLocationExist = await _context.CustomerLocations
                //    .AnyAsync(x => x.CustomerLocationId == model.CustomerLocationId);

                //if (!customerLocationExist)
                //{
                //    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                //}

                //if (!model.SerialNumber.HasValue())
                //{
                //    return Result.Fail($"{nameof(CustomerDeviceTokenValidateTokenModel.SerialNumber)} is required.");
                //}

                if (!model.DeviceToken.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerDeviceTokenValidateTokenModel.DeviceToken)} is required.");
                }

                //var token = await _context.CustomerDevices
                //    .Include(x => x.CustomerDeviceTokens)
                //    .SingleOrDefaultAsync(x => x.CustomerLocationId == model.CustomerLocationId
                //        && x.SerialNumber == model.SerialNumber);

                var entity = await _context.CustomerDeviceTokens
                    .Include(x => x.CustomerDevice)
                    .SingleOrDefaultAsync(x => x.DeviceToken.Trim().ToLower() == model.DeviceToken.Trim().ToLower());

                if (entity is null)
                {
                    return Result.Fail("Device token not found.");
                }
                else if (!entity.CustomerDevice.IsActive)
                {
                    return Result.Fail("Device is deactivated.");
                }
                else if (entity.IsValidated && entity.IsActive)
                {
                    return Result.Fail("This device token is already in used. Please contact your administrator to request a new one.");
                }
                else if (!entity.IsValidated && !entity.IsActive)
                {
                    return Result.Fail("This device token is deactivated.");
                }
                else if (entity.IsValidated && !entity.IsActive)
                {
                    return Result.Fail("This device token is deactivated.");
                }
                else
                {
                    entity.IsValidated = true;
                    entity.ValidatedDateTime = DateTime.Now;

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
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerDeviceTokenGetModel>> DeactivateCustomerDeviceTokenAsync(CustomerDeviceTokenAuthModel state,
            CustomerDeviceTokenValidateTokenModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (!model.DeviceToken.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerDeviceTokenValidateTokenModel.DeviceToken)} is required.");
                }

                var entity = await _context.CustomerDeviceTokens
                    .Include(x => x.CustomerDevice)
                    .SingleOrDefaultAsync(x => x.DeviceToken.Trim().ToLower() == model.DeviceToken.Trim().ToLower()
                        && x.CustomerDevice.CustomerLocationId == state.CustomerLocationId);

                if (entity is null)
                {
                    return Result.Fail("Device token not found.");
                }
                else if (!entity.IsValidated && !entity.IsActive)
                {
                    return Result.Fail("This device token is already deactivated.");
                }
                else if (entity.IsValidated && !entity.IsActive)
                {
                    return Result.Fail("This device token is already deactivated.");
                }
                else if (!entity.IsValidated && entity.IsActive)
                {
                    return Result.Fail("This device token is not validated yet. So it cannot deactivate.");
                }
                else // true, true
                {
                    entity.IsActive = false;
                    entity.DeactivedDateTime = DateTime.Now;

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
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<UserTemporaryTokenModel>> ResetPasswordRequestAsync(ResetPasswordModel model)
        {
            try
            {
                return await CreateTemporaryResetPasswordToken(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<string>> ResetPasswordAsync(string email, string password)
        {
            try
            {
                if (!password.HasValue())
                {
                    return Result.Fail("Must provide a new password.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return Result.Fail("Account not found. Please contact support.");
                }

                var newPass = CryptographicHelper.EncryptPassword(password);

                user.Password = newPass.Hash;
                user.PasswordSalt = newPass.Salt;

                await _context.SaveChangesAsync();

                return Result.Ok("Successfully reset password.");
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }
    }
}
