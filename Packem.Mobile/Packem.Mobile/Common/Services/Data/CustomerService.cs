using Packem.Domain.Models;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Services.Data
{
    public class CustomerService : ICustomerService
    {
        private readonly IHttpService _httpService;

        public CustomerService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<CustomerGetCurrentMobileModel>> GetCurrentCustomerForDeviceAsync(CustomerDeviceTokenAuthModel state)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.CustomerApi.GetCurrentCustomerForDevice;
            return await _httpService.GetAsync<CustomerGetCurrentMobileModel>(url, headers);
        }
    }
}
