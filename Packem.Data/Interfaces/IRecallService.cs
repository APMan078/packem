using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IRecallService
    {
        Task<Result<RecallGetModel>> CreateRecallAsync(AppState state, RecallCreateModel model);
        Task<Result<IEnumerable<RecallQueueGetModel>>> GetRecallQueueAsync(AppState state, int customerLocationId, int customerFacilityId);
        Task<Result<RecallGetModel>> UpdateRecallStatusDeviceAsync(CustomerDeviceTokenAuthModel state, RecallStatusUpdateModel model);
        Task<Result<IEnumerable<RecallQueueLookupGetModel>>> GetRecallQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false);
        Task<Result<IEnumerable<RecallDetailGetModel>>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId);
        Task<Result<RecallDetailGetModel>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId, int binId);
        Task<Result<RecallBinGetModel>> CreateRecallBinDeviceAsync(CustomerDeviceTokenAuthModel state, RecallBinCreateModel model);
    }
}