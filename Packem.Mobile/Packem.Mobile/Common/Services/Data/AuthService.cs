using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Services.Data
{
    public class AuthService : IAuthService
    {
        private readonly IHttpService _httpService;

        public AuthService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<HttpResponseWrapper<string>> Test()
        {
            var url = "http://192.168.192.250:5000/api/auth/test";
            var _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await response.Content.ReadAsStringAsync();
                return new HttpResponseWrapper<string>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<string>(default, false, response);
            }
        }

        public async Task<HttpResponseWrapper<UserTokenModel>> AuthenticateUserAsync(UserLoginModel model)
        {
            var url = App.AppSettings.PackemApiEndpoint + Constants.AuthApi.PostRequestUserToken;
            return await _httpService.PostAsync<UserTokenModel>(url, JsonConvert.SerializeObject(model));
        }

        public async Task<HttpResponseWrapper<AppState>> UserTokenInfoAsync(string jwtToken)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"Bearer {jwtToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.AuthApi.GetUserTokenInfo;
            return await _httpService.GetAsync<AppState>(url, headers);
        }

        public async Task<HttpResponseWrapper<CustomerDeviceTokenGetModel>> ValidateCustomerDeviceTokenAsync(CustomerDeviceTokenValidateTokenModel model)
        {
            var url = App.AppSettings.PackemApiEndpoint + Constants.AuthApi.PostValidateCustomerDeviceToken;
            return await _httpService.PostAsync<CustomerDeviceTokenGetModel>(url, JsonConvert.SerializeObject(model));
        }

        public async Task<HttpResponseWrapper<CustomerDeviceTokenAuthModel>> CustomerDeviceTokenInfoAsync(string deviceToken)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {deviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.AuthApi.GetCustomerDeviceTokenInfo;
            return await _httpService.GetAsync<CustomerDeviceTokenAuthModel>(url, headers);
        }

        public async Task<HttpResponseWrapper<CustomerDeviceTokenGetModel>> DeactivateCustomerDeviceTokenAsync(CustomerDeviceTokenValidateTokenModel model, string deviceToken)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"DeviceToken {deviceToken}" }
            };

            var url = App.AppSettings.PackemApiEndpoint + Constants.AuthApi.DeactivateCustomerDeviceToken;
            return await _httpService.PostAsync<CustomerDeviceTokenGetModel>(url, headers, JsonConvert.SerializeObject(model));
        }
    }
}
