using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<Result<PurchaseOrderGetModel>> CreatePurchaseOrderAsync(AppState state, PurchaseOrderCreateModel model);
        Task<Result<PurchaseOrderImportModel>> AddImportedPurchaseOrdersAsync(AppState state, int customerLocationId, PurchaseOrderImportModel[] model);
        Task<Result<PurchaseOrderLookupGetModel>> GetPurchaseOrderLookupAsync(AppState state, int customerFacilityId, string searchText);
        Task<Result<PurchaseOrderLookupGetModel>> DeletePurchaseOrderAsync(AppState state, PurchaseOrderDeleteModel model);
        Task<Result<PurchaseOrderDetailGetModel>> GetPurchaseOrderDetailAsync(AppState state, int purchaseOrderId);
        Task<Result<IEnumerable<PurchaseOrderWithVendorGetModel>>> GetPurchaseOrderByCustomerIdAsync(AppState state);
        Task<Result<IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel>>> GetPurchaseOrderLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
    }
}