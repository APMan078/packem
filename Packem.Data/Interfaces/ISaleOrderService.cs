using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ISaleOrderService
    {
        Task<Result<SaleOrderGetModel>> CreateSaleOrderAsync(AppState state, SaleOrderCreateModel model);
        Task<Result<SaleOrderAllGetModel>> GetSaleOrderAllAsync(AppState state, int customerLocationId, int customerFacilityId);
        Task<Result<SaleOrderDetailGetModel>> GetSaleOrderDetailAsync(AppState state, int saleOrderId);
        Task<Result<SaleOrderGetModel>> UpdateSaleOrderStatusToPrintedAsync(AppState state, int saleOrderId);
        Task<Result<SaleOrderPrintGetModel>> GetSaleOrderPrintAsync(AppState state, int saleOrderId);
        Task<Result<List<SaleOrderPrintGetModel>>> GetSaleOrderPrintMultipleAsync(AppState state, int[] saleOrderIds);
        Task<Result<SaleOrderPickQueueGetModel>> GetSaleOrderPickQueueAsync(AppState state, int customerLocationId, int customerFacilityId);
        Task<Result<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>>> GetSaleOrderPickQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
        Task<Result<SaleOrderGetModel>> UpdateSaleOrderPickingStatusDeviceAsync(CustomerDeviceTokenAuthModel state, SaleOrderPickingStatusUpdateModel model);
        Task<Result<PurchaseOrderImportModel>> AddImportedSaleOrdersAsync(AppState state, int customerLocationId, SalesOrderImportModel[] model);
    }
}