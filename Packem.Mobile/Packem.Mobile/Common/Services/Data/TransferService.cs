using Newtonsoft.Json;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Models;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Packem.Mobile.Common.Services.Data
{
    public class TransferService : ITransferService
    {
        private readonly IHttpService _httpService;

        public TransferService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<TransferLookupDeviceGetModel>>> GetTransferLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.TransferApi.GetTransferLookupDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && skuSearch)
            {
                url += $"&skuSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<TransferLookupDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<TransferGetModel>> CreateTransferManualDeviceAsync(CustomerDeviceTokenAuthModel state, TransferManualCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.TransferApi.CreateTransferManualDevice;
            return await _httpService.PostAsync<TransferGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }

        public async Task<HttpResponseWrapper<TransferGetModel>> CreateTransferRequestDeviceAsync(CustomerDeviceTokenAuthModel state, TransferRequestCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.TransferApi.CreateTransferRequestDevice;
            return await _httpService.PostAsync<TransferGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}