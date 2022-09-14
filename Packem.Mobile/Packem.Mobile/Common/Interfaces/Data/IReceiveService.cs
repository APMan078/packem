using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IReceiveService
    {
        Task<HttpResponseWrapper<IEnumerable<ReceiveLookupPOReceiveDeviceGetModel>>> GetReceiveLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int purchaseOrderId, string searchText, bool skuSearch = false);
        Task<HttpResponseWrapper<ReceiveGetModel>> UpdateReceiveStatusToReceivedDeviceAsync(CustomerDeviceTokenAuthModel state, int receiveId);
    }
}