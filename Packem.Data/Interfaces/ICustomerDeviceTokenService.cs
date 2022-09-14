using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ICustomerDeviceTokenService
    {
        Task<Result<CustomerDeviceTokenGetModel>> CreateCustomerDeviceTokenAsync(AppState state, CustomerDeviceTokenCreateModel model);
        Task<Result<IEnumerable<CustomerDeviceTokenGetModel>>> GetCustomerDeviceTokensByCustomerDeviceIdAsync(AppState state, int customerDeviceId);
        Task<Result<CustomerDeviceTokenGetModel>> GetCustomerDeviceTokenAsync(AppState state, int customerDeviceTokenId);
    }
}
