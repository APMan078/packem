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
    public class RecallService : IRecallService
    {
        private readonly IHttpService _httpService;

        public RecallService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<RecallGetModel>> UpdateRecallStatusDeviceAsync(CustomerDeviceTokenAuthModel state, RecallStatusUpdateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.RecallApi.UpdateRecallStatusDevice;
            return await _httpService.PostAsync<RecallGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }

        public async Task<HttpResponseWrapper<IEnumerable<RecallQueueLookupGetModel>>> GetRecallQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.RecallApi.GetRecallQueueLookupDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && barcodeSearch)
            {
                url += $"&barcodeSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<RecallQueueLookupGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<RecallDetailGetModel>>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.RecallApi.GetRecallDetailDevice + $"/{recallId}";
            return await _httpService.GetAsync<IEnumerable<RecallDetailGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<RecallDetailGetModel>> GetRecallDetailDeviceAsync(CustomerDeviceTokenAuthModel state, int recallId, int binId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.RecallApi.GetRecallDetailDevice2 + $"/{recallId}/{binId}";
            return await _httpService.GetAsync<RecallDetailGetModel>(url, headers);
        }

        public async Task<HttpResponseWrapper<RecallBinGetModel>> CreateRecallBinDeviceAsync(CustomerDeviceTokenAuthModel state, RecallBinCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.RecallApi.CreateRecallBinDevice;
            return await _httpService.PostAsync<RecallBinGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}