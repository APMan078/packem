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
    public class BinService : IBinService
    {
        private readonly IHttpService _httpService;

        public BinService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<BinZoneDeviceGetModel>>> GetBinZoneDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.BinApi.GetBinZoneDevice + $"/{customerFacilityId}/{itemId}";

            return await _httpService.GetAsync<IEnumerable<BinZoneDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<BinLookupDeviceGetModel>>> GetBinLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.BinApi.GetBinLookupDevice + $"/{zoneId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<BinLookupDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<BinLookupItemQuantityLotDeviceGetModel>>> GetBinItemQuantityLotLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, int itemId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.BinApi.GetBinItemQuantityLotLookupDevice + $"/{zoneId}/{itemId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<BinLookupItemQuantityLotDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<BinLookupItemQuantityGetModel>>> GetBinLookupItemQuantityDevice(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, int zoneId, string searchText, bool barcodeSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.BinApi.GetBinLookupItemQuantityDevice + $"/{customerFacilityId}/{itemId}/{zoneId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && barcodeSearch)
            {
                url += $"&barcodeSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<BinLookupItemQuantityGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<BinLookupItemQuantityGetModel>> GetBinItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, int binId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.BinApi.GetBinItemQuantityDevice + $"/{itemId}/{binId}";

            return await _httpService.GetAsync<BinLookupItemQuantityGetModel>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<BinLookupOptionalPalletDeviceGetModel>>> GetBinLookupOptionalPalletDeviceAsync(CustomerDeviceTokenAuthModel state, int zoneId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.BinApi.GetBinLookupOptionalPalletDevice + $"/{zoneId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<BinLookupOptionalPalletDeviceGetModel>>(url, headers);
        }
    }
}