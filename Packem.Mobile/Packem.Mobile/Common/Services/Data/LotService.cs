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
    public class LotService : ILotService
    {
        private readonly IHttpService _httpService;

        public LotService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<LotLookupGetModel>>> GetLotLookupByItemIdDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.LotApi.GetLotLookupDevice + $"/{itemId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<LotLookupGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<LotGetModel>> CreateLotDeviceAsync(CustomerDeviceTokenAuthModel state, LotCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.LotApi.CreateLotDevice;
            return await _httpService.PostAsync<LotGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}