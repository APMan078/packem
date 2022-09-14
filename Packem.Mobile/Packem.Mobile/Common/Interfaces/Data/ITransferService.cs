using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface ITransferService
    {
        Task<HttpResponseWrapper<IEnumerable<TransferLookupDeviceGetModel>>> GetTransferLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false);
        Task<HttpResponseWrapper<TransferGetModel>> CreateTransferManualDeviceAsync(CustomerDeviceTokenAuthModel state, TransferManualCreateModel model);
        Task<HttpResponseWrapper<TransferGetModel>> CreateTransferRequestDeviceAsync(CustomerDeviceTokenAuthModel state, TransferRequestCreateModel model);
    }
}