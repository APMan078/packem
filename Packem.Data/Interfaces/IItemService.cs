using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IItemService
    {
        Task<Result<ItemGetModel>> CreateItemAsync(AppState state, ItemCreateModel model);
        Task<Result<ItemGetModel>> DeleteItemAsync(AppState state, ItemDeleteModel model);
        Task<Result<ItemGetModel>> UpdateItemExpirationDateAsync(AppState state, ItemExpirationDateUpdateModel model);
        Task<Result<ItemGetModel>> UpdateItemThresholdAsync(AppState state, ItemThresholdUpdateModel model);
        Task<Result<IEnumerable<ItemLookupGetModel>>> GetItemLookupAsync(AppState state, int customerId, string searchText);
        Task<Result<IEnumerable<ItemLookupDetailBasicGetModel>>> GetItemLookupDetailBasicAsync(AppState state, int customerId, string searchText);
        Task<Result<ItemDetailGetModel>> GetItemDetailAsync(AppState state, int customerId, int itemId);
        Task<Result<IEnumerable<ItemVendorGetModel>>> GetItemByVendorIdAsync(AppState state, int vendorId);
        Task<Result<IEnumerable<ItemSkuLookupGetModel>>> GetItemLookupBySkuAsync(AppState state, int customerId, string searchText);
        Task<Result<ItemSkuLookupGetModel>> GetItemSkuLookupAsync(AppState state, int customerId, string sku);
        Task<Result<IEnumerable<ItemManualReceiptLookupGetModel>>> GetItemManualReceiptLookupAsync(AppState state, int customerId, string searchText);
        Task<Result<IEnumerable<ItemPurchaseOrderLookupGetModel>>> GetItemPurchaseOrderLookupAsync(AppState state, int purchaseOrderId, string searchText);
        Task<Result<IEnumerable<ItemLookupDeviceGetModel>>> GetItemLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText);
        Task<Result<ItemLookupDeviceGetModel>> GetItemLookupSkuDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string sku);
    }
}