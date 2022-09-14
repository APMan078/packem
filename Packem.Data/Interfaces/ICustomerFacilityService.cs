using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ICustomerFacilityService
    {
        Task<Result<CustomerFacilityGetModel>> CreateCustomerFacilityAsync(AppState state, CustomerFacilityCreateModel model);
        Task<Result<CustomerFacilityGetModel>> EditCustomerFacilityAsync(AppState state, CustomerFacilityEditModel model);
        Task<Result<CustomerFacilityGetModel>> DeleteCustomerFacilityAsync(AppState state, CustomerFacilityDeleteModel model);
        Task<Result<IEnumerable<CustomerFacilityGetModel>>> GetCustomerLocationsSuperAdminAsync();
        Task<Result<IEnumerable<CustomerFacilityGetModel>>> GetCustomerFacilitiesByCustomerIdAsync(AppState state, int customerId);
        Task<Result<IEnumerable<CustomerFacilityGetModel>>> GetCustomerFacilitiesByCustomerLocationIdAsync(AppState state, int customerLocationId);
        Task<Result<CustomerFacilityGetModel>> GetCustomerFacilityAsync(AppState state, int customerFacilityId);
    }
}