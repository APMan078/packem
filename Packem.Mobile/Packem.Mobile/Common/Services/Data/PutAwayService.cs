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
    public class PutAwayService : IPutAwayService
    {
        private readonly IHttpService _httpService;

        public PutAwayService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<PutAwayGetModel>> CreatePutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PutAwayApi.CreatePutAwayDevice;
            return await _httpService.PostAsync<PutAwayGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }

        public async Task<HttpResponseWrapper<PutAwayBinGetModel>> CreatePutAwayBinDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayBinDeviceCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PutAwayApi.CreatePutAwayBinDevice;
            return await _httpService.PostAsync<PutAwayBinGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }

        public async Task<HttpResponseWrapper<IEnumerable<PutAwayLookupDeviceGetModel>>> GetPutAwayLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool skuSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PutAwayApi.GetPutAwayLookupDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && skuSearch)
            {
                url += $"&skuSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<PutAwayLookupDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<PutAwayLookupDeviceGetModel>> GetPutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, int putAwayId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PutAwayApi.GetPutAwayDevice + $"/{putAwayId}";

            return await _httpService.GetAsync<PutAwayLookupDeviceGetModel>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<PutAwayLookupLicensePlateDeviceGetModel>>> GetPutAwayLookupLicensePlateDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PutAwayApi.GetPutAwayLookupLicensePlateDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && barcodeSearch)
            {
                url += $"&barcodeSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<PutAwayLookupLicensePlateDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<PutAwayBinGetModel>>> CreatePutAwayPalletDeviceAsync(CustomerDeviceTokenAuthModel state, PutAwayPalletDeviceCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PutAwayApi.CreatePutAwayPalletDevice;
            return await _httpService.PostAsync<IEnumerable<PutAwayBinGetModel>>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}