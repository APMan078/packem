using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Services.Data
{
    public class ReceiptService : IReceiptService
    {
        private readonly IHttpService _httpService;

        public ReceiptService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<ReceiptGetModel>> CreateReceiptDeviceAsync(CustomerDeviceTokenAuthModel state, ReceiptDeviceCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.ReceiptApi.CreateReceiptDevice;
            return await _httpService.PostAsync<ReceiptGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}