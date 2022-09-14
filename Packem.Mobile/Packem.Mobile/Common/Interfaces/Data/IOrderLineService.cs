using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IOrderLineService
    {
        Task<HttpResponseWrapper<OrderLineGetModel>> GetOrderLineDeviceAsync(CustomerDeviceTokenAuthModel state, int orderLineId);
        Task<HttpResponseWrapper<IEnumerable<OrderLinePickLookupGetModel>>> GetOrderLinePickLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int saleOrderId, string searchText, bool barcodeSearch = false);
        Task<HttpResponseWrapper<OrderLineBinGetModel>> CreateOrderLineBinDeviceAsync(CustomerDeviceTokenAuthModel state, OrderLineBinCreateModel model);
    }
}