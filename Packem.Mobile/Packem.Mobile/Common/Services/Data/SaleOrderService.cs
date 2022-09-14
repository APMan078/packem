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
    public class SaleOrderService : ISaleOrderService
    {
        private readonly IHttpService _httpService;

        public SaleOrderService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>>> GetSaleOrderPickQueueLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.SaleOrderApi.GetSaleOrderPickQueueLookupDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && barcodeSearch)
            {
                url += $"&barcodeSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<SaleOrderGetModel>> UpdateSaleOrderPickingStatusDeviceAsync(CustomerDeviceTokenAuthModel state, SaleOrderPickingStatusUpdateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.SaleOrderApi.UpdateSaleOrderPickingStatusDevice;
            return await _httpService.PostAsync<SaleOrderGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}