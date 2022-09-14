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
    public class OrderLineService : IOrderLineService
    {
        private readonly IHttpService _httpService;

        public OrderLineService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<OrderLineGetModel>> GetOrderLineDeviceAsync(CustomerDeviceTokenAuthModel state, int orderLineId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.OrderLineApi.GetOrderLineDevice + $"/{orderLineId}";
            return await _httpService.GetAsync<OrderLineGetModel>(url, headers);
        }

        public async Task<HttpResponseWrapper<IEnumerable<OrderLinePickLookupGetModel>>> GetOrderLinePickLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int saleOrderId, string searchText, bool barcodeSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.OrderLineApi.GetOrderLinePickLookupDevice + $"/{saleOrderId}";

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && barcodeSearch)
            {
                url += $"&barcodeSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<OrderLinePickLookupGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<OrderLineBinGetModel>> CreateOrderLineBinDeviceAsync(CustomerDeviceTokenAuthModel state, OrderLineBinCreateModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.OrderLineApi.CreateOrderLineBinDevice;
            return await _httpService.PostAsync<OrderLineBinGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}