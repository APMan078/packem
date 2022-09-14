using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface ICustomerService
    {
        Task<HttpResponseWrapper<CustomerGetCurrentMobileModel>> GetCurrentCustomerForDeviceAsync(CustomerDeviceTokenAuthModel state);
    }
}