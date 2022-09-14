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
    public class ZoneService : IZoneService
    {
        private readonly IHttpService _httpService;

        public ZoneService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<ZoneLookupDeviceGetModel>>> GetZoneLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.ZoneApi.GetZoneLookupDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<ZoneLookupDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<ZoneLookupItemQuantityGetModel>>> GetZoneLookupItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.ZoneApi.GetZoneLookupItemQuantityDevice + $"/{customerFacilityId}/{itemId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<ZoneLookupItemQuantityGetModel>>(url, headers);
        }
    }
}
