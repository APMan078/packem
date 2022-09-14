using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IRecallService
    {
        Task<HttpResponseWrapper<RecallGetModel>> UpdateRecallStatusDeviceAsync(CustomerDeviceTokenAuthModel state, RecallStatusUpdateModel model);
        Task<HttpResponseWrapper<IEnumerable<RecallQueueLookupGetModel>>> GetRecallQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
        Task<HttpResponseWrapper<IEnumerable<RecallDetailGetModel>>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId);
        Task<HttpResponseWrapper<RecallDetailGetModel>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId, int binId);
        Task<HttpResponseWrapper<RecallBinGetModel>> CreateRecallBinDeviceAsync(CustomerDeviceTokenAuthModel state, RecallBinCreateModel model);
    }
}