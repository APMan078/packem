using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface ILotService
    {
        Task<HttpResponseWrapper<IEnumerable<LotLookupGetModel>>> GetLotLookupByItemIdDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText);
        Task<HttpResponseWrapper<LotGetModel>> CreateLotDeviceAsync(CustomerDeviceTokenAuthModel state, LotCreateModel model);
    }
}