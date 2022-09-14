using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ICustomerService
    {
        Task<Result<CustomerGetModel>> CreateCustomerAsync(CustomerCreateModel model);
        Task<Result<CustomerGetModel>> EditCustomerAsync(CustomerEditModel model);
        Task<Result<CustomerGetModel>> EditCustomerIsActiveAsync(AppState state, CustomerIsActiveEditModel model);
        Task<Result<IEnumerable<CustomerGetModel>>> GetCustomersAsync();
        Task<Result<CustomerGetModel>> GetCustomerAsync(int customerId);
        Task<Result<int>> GetCustomerDefaultThresholdAsync(int customerId);
        Task<Result<CustomerGetModel>> UpdateCustomerDefaultThresholdAsync(int customerId, int? threshold);
        Task<Result<CustomerGetCurrentModel>> GetCurrentCustomerAsync(AppState state);
        Task<Result<CustomerGetCurrentMobileModel>> GetCurrentCustomerForDeviceAsync(CustomerDeviceTokenAuthModel state);
    }
}
