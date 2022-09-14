using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ITransferService
    {
        Task<Result<TransferGetModel>> CreateTransferAsync(AppState state, TransferCreateModel model);
        Task<Result<TransferHistoryGetModel>> GetTransferHistoryAsync(AppState state, int customerLocationId, int customerFacilityId);
        Task<Result<IEnumerable<TransferLookupDeviceGetModel>>> GetTransferLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false);
        Task<Result<TransferGetModel>> CreateTransferManualDeviceAsync(CustomerDeviceTokenAuthModel state, TransferManualCreateModel model);
        Task<Result<TransferGetModel>> CreateTransferRequestDeviceAsync(CustomerDeviceTokenAuthModel state, TransferRequestCreateModel model);
    }
}