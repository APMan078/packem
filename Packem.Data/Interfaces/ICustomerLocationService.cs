using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ICustomerLocationService
    {
        Task<Result<CustomerLocationGetModel>> CreateCustomerLocationAsync(AppState state, CustomerLocationCreateModel model);
        Task<Result<CustomerLocationGetModel>> EditCustomerLocationAsync(AppState state, CustomerLocationEditModel model);
        Task<Result<CustomerLocationGetModel>> DeleteCustomerLocationAsync(AppState state, CustomerLocationDeleteModel model);
        Task<Result<IEnumerable<CustomerLocationGetModel>>> GetCustomerLocationsSuperAdminAsync();
        Task<Result<IEnumerable<CustomerLocationGetModel>>> GetCustomerLocationsByCustomerIdAsync(int customerId);
        Task<Result<CustomerLocationGetModel>> GetCustomerLocationAsync(AppState state, int customerLocationId);
    }
}