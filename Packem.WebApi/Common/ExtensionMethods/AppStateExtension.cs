using Packem.Data.Models;
using Packem.Domain.Common.Enums;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Models;
using System;
using System.Security.Claims;

namespace Packem.WebApi.Common.ExtensionMethods
{
    public static class AppStateExtension
    {
        public static AppState ToAppState(this ClaimsPrincipal user)
        {
            var state = new AppState
            {
                UserId = Convert.ToInt32(user.FindFirstValue(CustomClaim.Claim_UserId)),
                Role = (RoleEnum)Convert.ToInt32(user.FindFirstValue(ClaimTypes.Role)),
                CustomerLocationId = user.FindFirstValue(CustomClaim.Claim_CustomerLocationId).ToNullableInt(),
                CustomerId = user.FindFirstValue(CustomClaim.Claim_CustomerId).ToNullableInt(),
                CustomerName = user.FindFirstValue(CustomClaim.Claim_CustomerName)
            };

            return state;
        }

        public static CustomerDeviceTokenAuthModel ToCustomerDeviceTokenAuth(this ClaimsPrincipal user)
        {
            var model = new CustomerDeviceTokenAuthModel
            {
                CustomerDeviceTokenId = Convert.ToInt32(user.FindFirstValue(ClaimTypes.NameIdentifier)),
                DeviceToken = user.FindFirstValue(ClaimTypes.Name),
                CustomerId = Convert.ToInt32(user.FindFirstValue(CustomClaim.Claim_CustomerId)),
                CustomerLocationId = Convert.ToInt32(user.FindFirstValue(CustomClaim.Claim_CustomerLocationId))
            };

            return model;
        }
    }
}
