using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Packem.Data;
using Packem.Data.Models;
using Packem.Domain.Entities;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Packem.WebApi.Common.CustomProviders
{
    public class DeviceTokenAuthOptions : AuthenticationSchemeOptions
    {
        public const string DeviceTokenScemeName = "DeviceToken";
    }

    public class DeviceTokenAuthHandler : AuthenticationHandler<DeviceTokenAuthOptions>
    {
        private readonly ApplicationDbContext _context;

        public DeviceTokenAuthHandler(IOptionsMonitor<DeviceTokenAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApplicationDbContext context)
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                try
                {
                    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                    if (authHeader.Scheme == DeviceTokenAuthOptions.DeviceTokenScemeName)
                    {
                        CustomerDeviceToken token = null;

                        token = await _context.CustomerDeviceTokens
                            .AsNoTracking()
                            .Include(x => x.CustomerDevice)
                                .ThenInclude(x => x.CustomerLocation)
                            .SingleOrDefaultAsync(x => !string.IsNullOrWhiteSpace(authHeader.Parameter)
                                && x.DeviceToken.Trim().ToLower() == authHeader.Parameter.Trim().ToLower()
                                && x.IsValidated && x.IsActive
                                && x.CustomerDevice.IsActive);

                        if (token == null)
                            return AuthenticateResult.Fail("Invalid Token.");

                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, token.CustomerDeviceTokenId.ToString()),
                            new Claim(ClaimTypes.Name, token.DeviceToken),
                        };

                        if (token?.CustomerDevice?.CustomerLocation?.CustomerId is not null)
                        {
                            claims.Add(new Claim(CustomClaim.Claim_CustomerId, token.CustomerDevice.CustomerLocation.CustomerId.ToString()));
                        }

                        if (token?.CustomerDevice?.CustomerLocationId is not null)
                        {
                            claims.Add(new Claim(CustomClaim.Claim_CustomerLocationId, token.CustomerDevice.CustomerLocationId.ToString()));
                        }

                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    }
                }
                catch
                {
                    return AuthenticateResult.Fail("Authorization Failed.");
                }
            }

            return AuthenticateResult.NoResult();
        }
    }
}