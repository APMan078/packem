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
    public class LicensePlateService : ILicensePlateService
    {
        private readonly IHttpService _httpService;

        public LicensePlateService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<IEnumerable<LicensePlateLookupDeviceGetModel>>> GetLicensePlateLookupDeviceAsync(CustomerDeviceTokenAuthModel state, string searchText, bool barcodeSearch = false)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.LicensePlateApi.GetLicensePlateLookupDevice;

            if (searchText.HasValue())
            {
                url += $"?searchText={HttpUtility.UrlEncode(searchText)}";
            }

            if (searchText.HasValue() && barcodeSearch)
            {
                url += $"&barcodeSearch=true";
            }

            return await _httpService.GetAsync<IEnumerable<LicensePlateLookupDeviceGetModel>>(url, headers);
        }

        public async Task<HttpResponseWrapper<LicensePlateKnownAssignmentDeviceGetModel>> GetLicensePlateKnownAssignmentDeviceAsync(CustomerDeviceTokenAuthModel state, int licensePlateId)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.LicensePlateApi.GetLicensePlateKnownAssignmentDevice + $"/{licensePlateId}";
            return await _httpService.GetAsync<LicensePlateKnownAssignmentDeviceGetModel>(url, headers);
        }

        public async Task<HttpResponseWrapper<LicensePlateGetModel>> EditLicensePlateUnknownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateUnknownToPalletizedEditModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.LicensePlateApi.EditLicensePlateUnknownToPalletizedDevice;
            return await _httpService.PostAsync<LicensePlateGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }

        public async Task<HttpResponseWrapper<LicensePlateGetModel>> EditLicensePlateKnownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateKnownToPalletizedEditModel model)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {state.DeviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.LicensePlateApi.EditLicensePlateKnownToPalletizedDevice;
            return await _httpService.PostAsync<LicensePlateGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}