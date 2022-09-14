using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IPutAwayService
    {
        Task<Result<PutAwayQueueGetModel>> GetPutAwayQueueAsync(AppState state, int customerLocationId, int customerFacilityId);
        Task<Result<PutAwayBinGetModel>> CreatePutAwayBinAsync(AppState state, PutAwayBinCreateModel model);
        Task<Result<PutAwayGetModel>> CreatePutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayCreateModel model);
        Task<Result<PutAwayBinGetModel>> CreatePutAwayBinDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayBinDeviceCreateModel model);
        Task<Result<IEnumerable<PutAwayLookupDeviceGetModel>>> GetPutAwayLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false);
        Task<Result<PutAwayLookupDeviceGetModel>> GetPutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, int putAwayId);
        Task<Result<IEnumerable<PutAwayLookupLicensePlateDeviceGetModel>>> GetPutAwayLookupLicensePlateDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
        Task<Result<IEnumerable<PutAwayBinGetModel>>> CreatePutAwayPalletDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayPalletDeviceCreateModel model);
    }
}