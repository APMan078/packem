using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IPurchaseOrderService
    {
        Task<HttpResponseWrapper<IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel>>> GetPurchaseOrderLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
        Task<HttpResponseWrapper<PurchaseOrderGetModel>> UpdatePurchaseOrderStatusToPutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, int purchaseOrderId);
    }
}