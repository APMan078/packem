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
    public class ItemService : IItemService
    {
        private readonly IHttpService _httpService;

        public ItemService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<ItemLookupDeviceGetModel>>> GetItemLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.ItemApi.GetItemLookupDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            return await _httpService.GetAsync<IEnumerable<ItemLookupDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<ItemLookupDeviceGetModel>> GetItemLookupSkuDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string sku)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.ItemApi.GetItemLookupSkuDevice + $"/{customerFacilityId}";

            if (sku.HasValue())
            {
                url += $"?sku={HttpUtility.UrlEncode(sku)}";
            }

            return await _httpService.GetAsync<ItemLookupDeviceGetModel>(url, headers);
        }
    }
}
