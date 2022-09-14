using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IOrderLineService
    {
        Task<Result<OrderLineGetModel>> CreateOrderLineAsync(AppState state, OrderLineCreateModel model);
        Task<Result<OrderLineGetModel>> UpdateOrderLineAsync(AppState state, OrderLineUpdateModel model);
        Task<Result<OrderLineGetModel>> DeleteOrderLineAsync(AppState state, OrderLineDeleteModel model);
        Task<Result<OrderLineGetModel>> GetOrderLineDeviceAsync(CustomerDeviceTokenAuthModel state, int orderLineId);
        Task<Result<IEnumerable<OrderLinePickLookupGetModel>>> GetOrderLinePickLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int saleOrderId, string searchText, bool barcodeSearch = false);
        Task<Result<OrderLineBinGetModel>> CreateOrderLineBinDeviceAsync(CustomerDeviceTokenAuthModel state, OrderLineBinCreateModel model);
    }
}