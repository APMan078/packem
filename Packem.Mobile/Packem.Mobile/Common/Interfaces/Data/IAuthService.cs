using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IAuthService
    {
        Task<HttpResponseWrapper<string>> Test();
        Task<HttpResponseWrapper<UserTokenModel>> AuthenticateUserAsync(UserLoginModel model);
        Task<HttpResponseWrapper<AppState>> UserTokenInfoAsync(string jwtToken);
        Task<HttpResponseWrapper<CustomerDeviceTokenGetModel>> ValidateCustomerDeviceTokenAsync(CustomerDeviceTokenValidateTokenModel model);
        Task<HttpResponseWrapper<CustomerDeviceTokenAuthModel>> CustomerDeviceTokenInfoAsync(string deviceToken);
        Task<HttpResponseWrapper<CustomerDeviceTokenGetModel>> DeactivateCustomerDeviceTokenAsync(CustomerDeviceTokenValidateTokenModel model, string deviceToken);
    }
}
