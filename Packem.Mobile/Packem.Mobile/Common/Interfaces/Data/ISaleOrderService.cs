using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface ISaleOrderService
    {
        Task<HttpResponseWrapper<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>>> GetSaleOrderPickQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
        Task<HttpResponseWrapper<SaleOrderGetModel>> UpdateSaleOrderPickingStatusDeviceAsync(CustomerDeviceTokenAuthModel state, SaleOrderPickingStatusUpdateModel model);
    }
}