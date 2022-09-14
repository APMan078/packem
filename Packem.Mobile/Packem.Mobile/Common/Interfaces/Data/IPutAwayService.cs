using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IPutAwayService
    {
        Task<HttpResponseWrapper<PutAwayGetModel>> CreatePutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayCreateModel model);
        Task<HttpResponseWrapper<PutAwayBinGetModel>> CreatePutAwayBinDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayBinDeviceCreateModel model);
        Task<HttpResponseWrapper<IEnumerable<PutAwayLookupDeviceGetModel>>> GetPutAwayLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false);
        Task<HttpResponseWrapper<PutAwayLookupDeviceGetModel>> GetPutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, int putAwayId);
        Task<HttpResponseWrapper<IEnumerable<PutAwayLookupLicensePlateDeviceGetModel>>> GetPutAwayLookupLicensePlateDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
        Task<HttpResponseWrapper<IEnumerable<PutAwayBinGetModel>>> CreatePutAwayPalletDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayPalletDeviceCreateModel model);
    }
}