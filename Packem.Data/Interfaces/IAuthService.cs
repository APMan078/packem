using FluentResults;
using Packem.Domain.Models;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IAuthService
    {
        Task<Result<UserTokenModel>> AuthenticateUserAsync(UserLoginModel model);
        Task<Result<CustomerDeviceTokenGetModel>> ValidateCustomerDeviceTokenAsync(CustomerDeviceTokenValidateTokenModel model);
        Task<Result<CustomerDeviceTokenGetModel>> DeactivateCustomerDeviceTokenAsync(CustomerDeviceTokenAuthModel state, CustomerDeviceTokenValidateTokenModel model);
        Task<Result<UserTemporaryTokenModel>> ResetPasswordRequestAsync(ResetPasswordModel model);
        Task<Result<string>> ResetPasswordAsync(string email, string password);
    }
}
