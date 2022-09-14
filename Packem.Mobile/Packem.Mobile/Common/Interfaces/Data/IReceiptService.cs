using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface IReceiptService
    {
        Task<HttpResponseWrapper<ReceiptGetModel>> CreateReceiptDeviceAsync(CustomerDeviceTokenAuthModel state, ReceiptDeviceCreateModel model);
    }
}