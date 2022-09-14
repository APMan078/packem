using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IReceiveService
    {
        Task<Result<ReceiveGetModel>> CreateReceiveAsync(AppState state, ReceiveCreateModel model);
        Task<Result<ReceiveGetModel>> UpdateReceiveQtyAsync(AppState state, ReceiveQtyUpdateModel model);
        Task<Result<ReceiveGetModel>> DeleteReceiveAsync(AppState state, ReceiveDeleteModel model);
        Task<Result<IEnumerable<ReceiveLookupPOReceiveDeviceGetModel>>> GetReceiveLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int purchaseOrderId, string searchText, bool skuSearch = false);
    }
}