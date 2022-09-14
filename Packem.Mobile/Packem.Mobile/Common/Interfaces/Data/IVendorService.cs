using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IVendorService
    {
        Task<HttpResponseWrapper<IEnumerable<VendorLookupNameGetModel>>> GetVendorLookupByNameDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText);
    }
}