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
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IHttpService _httpService;

        public PurchaseOrderService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel>>> GetPurchaseOrderLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText, bool barcodeSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PurchaseOrderApi.GetPurchaseOrderLookupDevice + $"/{customerFacilityId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && barcodeSearch)
            {
                url += $"&barcodeSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<PurchaseOrderGetModel>> UpdatePurchaseOrderStatusToPutAwayDeviceAsync(CustomerDeviceTokenAuthModel state, int purchaseOrderId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.PurchaseOrderApi.UpdatePurchaseOrderStatusToPutAwayDevice + $"/{purchaseOrderId}";

            return await _httpService.PutAsync<PurchaseOrderGetModel>(url, headers, null);
        }
    }
}