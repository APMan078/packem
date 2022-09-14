using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IZoneService
    {
        Task<HttpResponseWrapper<IEnumerable<ZoneLookupDeviceGetModel>>> GetZoneLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText);
        Task<HttpResponseWrapper<IEnumerable<ZoneLookupItemQuantityGetModel>>> GetZoneLookupItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, string searchText);
    }
}