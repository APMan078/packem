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
    public class VendorService : IVendorService
    {
        private readonly IHttpService _httpService;

        public VendorService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<VendorLookupNameGetModel>>> GetVendorLookupByNameDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.VendorApi.GetVendorLookupByNameDevice + $"/{itemId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<VendorLookupNameGetModel>>(url, headers);
        }
    }
}