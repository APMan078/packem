using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IItemService
    {
        Task<HttpResponseWrapper<IEnumerable<ItemLookupDeviceGetModel>>> GetItemLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText);
        Task<HttpResponseWrapper<ItemLookupDeviceGetModel>> GetItemLookupSkuDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string sku);
    }
}