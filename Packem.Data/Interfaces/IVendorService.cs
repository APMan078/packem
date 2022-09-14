using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IVendorService
    {
        Task<Result<VendorGetModel>> CreateVendorAsync(AppState state, VendorCreateModel model);
        Task<Result<VendorGetModel>> EditVendorAsync(AppState state, VendorEditModel model);
        Task<Result<IEnumerable<VendorItemGetModel>>> GetVendorItemsAsync(AppState state, int customerId);
        Task<Result<IEnumerable<VendorLookupGetModel>>> GetVendorLookupAsync(AppState state, int customerId, string searchText);
        Task<Result<IEnumerable<VendorLookupNameGetModel>>> GetVendorLookupByNameAsync(AppState state, int customerId, string searchText);
        Task<Result<IEnumerable<VendorLookupNameGetModel>>> GetVendorLookupByNameDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText);
    }
}
