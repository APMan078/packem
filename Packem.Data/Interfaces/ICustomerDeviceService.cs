using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ICustomerDeviceService
    {
        Task<Result<CustomerDeviceGetModel>> CreateCustomerDeviceAsync(AppState state, CustomerDeviceCreateModel model);
        Task<Result<CustomerDeviceGetModel>> EditCustomerDeviceAsync(AppState state, CustomerDeviceEditModel model);
        Task<Result<CustomerDeviceGetModel>> EditCustomerDeviceIsActiveAsync(AppState state, CustomerDeviceIsActiveEditModel model);
        Task<Result<IEnumerable<CustomerDeviceGetModel>>> GetCustomerDevicesByCustomerIdAsync(AppState state, int customerId);
        Task<Result<IEnumerable<CustomerDeviceGetModel>>> GetCustomerDevicesByCustomerLocationIdAsync(AppState state, int customerLocationId);
        Task<Result<CustomerDeviceGetModel>> GetCustomerDeviceAsync(AppState state, int customerDeviceId);
    }
}