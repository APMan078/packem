using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IBinService
    {
        Task<Result<BinGetModel>> CreateBinAsync(AppState state, BinCreateModel model);
        Task<Result<BinGetModel>> EditBinAsync(AppState state, BinEditModel model);
        Task<Result<IEnumerable<BinGetModel>>> GetBinByCustomerLocationIdAsync(AppState state, int customerLocationId);
        Task<Result<BinGetModel>> GetBinAsync(AppState state, int binId);
        Task<Result<IEnumerable<BinLookupGetModel>>> GetBinLookupAsync(AppState state, int zoneId, string searchText);
        Task<Result<IEnumerable<BinLookupItemQuantityGetModel>>> GetBinLookupItemQuantityAsync(AppState state, int itemId, int zoneId, string searchText);
        Task<Result<BinStorageManagementGetModel>> GetBinStorageManagementAsync(AppState state, int customerLocationId, int customerFacilityId);
        Task<Result<BinStorageManagementDetailGetModel>> GetBinStorageManagementDetailAsync(AppState state, int binId);
        Task<Result<IEnumerable<BinZoneDeviceGetModel>>> GetBinZoneDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId);
        Task<Result<IEnumerable<BinLookupDeviceGetModel>>> GetBinLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText);
        Task<Result<IEnumerable<BinLookupItemQuantityLotDeviceGetModel>>> GetBinItemQuantityLotLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, int itemId, string searchText);
        Task<Result<IEnumerable<BinLookupItemQuantityGetModel>>> GetBinLookupItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, int zoneId, string searchText, bool barcodeSearch = false);
        Task<Result<BinLookupItemQuantityGetModel>> GetBinItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, int binId);
        Task<Result<PurchaseOrderImportModel>> AddImportedZonesAndBins(AppState state, int customerLocationId, ZoneAndBinImportModel[] model);
        Task<Result<IEnumerable<BinLookupOptionalPalletDeviceGetModel>>> GetBinLookupOptionalPalletDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText);
    }
}