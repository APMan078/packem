using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IBinService
    {
        Task<HttpResponseWrapper<IEnumerable<BinZoneDeviceGetModel>>> GetBinZoneDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId);
        Task<HttpResponseWrapper<IEnumerable<BinLookupDeviceGetModel>>> GetBinLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText);
        Task<HttpResponseWrapper<IEnumerable<BinLookupItemQuantityLotDeviceGetModel>>> GetBinItemQuantityLotLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, int itemId, string searchText);
        Task<HttpResponseWrapper<IEnumerable<BinLookupItemQuantityGetModel>>> GetBinLookupItemQuantityDevice(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, int zoneId, string searchText, bool barcodeSearch = false);
        Task<HttpResponseWrapper<BinLookupItemQuantityGetModel>> GetBinItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, int binId);
        Task<HttpResponseWrapper<IEnumerable<BinLookupOptionalPalletDeviceGetModel>>> GetBinLookupOptionalPalletDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText);
    }
}