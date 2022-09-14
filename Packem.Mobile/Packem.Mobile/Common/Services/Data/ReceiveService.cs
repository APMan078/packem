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
    public class ReceiveService : IReceiveService
    {
        private readonly IHttpService _httpService;

        public ReceiveService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<ReceiveLookupPOReceiveDeviceGetModel>>> GetReceiveLookupPOReceiveDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int purchaseOrderId, string searchText, bool skuSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.ReceiveApi.GetPurchaseOrderLookupPOReceiveDevice + $"/{customerFacilityId}/{purchaseOrderId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && skuSearch)
            {
                url += $"&skuSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<ReceiveLookupPOReceiveDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<ReceiveGetModel>> UpdateReceiveStatusToReceivedDeviceAsync(CustomerDeviceTokenAuthModel state, int receiveId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.ReceiveApi.UpdateReceiveStatusToReceivedDevice + $"/{receiveId}";

            return await _httpService.PutAsync<ReceiveGetModel>(url, headers, null);
        }
    }
}
