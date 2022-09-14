using Newtonsoft.Json;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Services.General
{
    public class HttpService : IHttpService
    {
        private async Task<T> Deserialize<T>(HttpResponseMessage httpResponse)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<T>(response);
                return new HttpResponseWrapper<T>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, response);
            }
        }

        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url, Dictionary<string, string> headers)
        {
            var httpClient = new HttpClient();
            foreach (KeyValuePair<string, string> x in headers)
            {
                httpClient.DefaultRequestHeaders.Add(x.Key, x.Value);
            }

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<T>(response);
                return new HttpResponseWrapper<T>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, response);
            }
        }

        public async Task<HttpResponseWrapper<T>> PostAsync<T>(string url, string jsonData)
        {
            var httpClient = new HttpClient();

            StringContent stringContent = null;
            if (jsonData.HasValue())
            {
                stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.PostAsync(url, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<T>(response);
                return new HttpResponseWrapper<T>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, response);
            }
        }

        public async Task<HttpResponseWrapper<T>> PostAsync<T>(string url, Dictionary<string, string> headers, string jsonData)
        {
            var httpClient = new HttpClient();
            foreach (KeyValuePair<string, string> x in headers)
            {
                httpClient.DefaultRequestHeaders.Add(x.Key, x.Value);
            }

            StringContent stringContent = null;
            if (jsonData.HasValue())
            {
                stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.PostAsync(url, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<T>(response);
                return new HttpResponseWrapper<T>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, response);
            }
        }

        public async Task<HttpResponseWrapper<T>> PutAsync<T>(string url, string jsonData)
        {
            var httpClient = new HttpClient();

            StringContent stringContent = null;
            if (jsonData.HasValue())
            {
                stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.PutAsync(url, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<T>(response);
                return new HttpResponseWrapper<T>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, response);
            }
        }

        public async Task<HttpResponseWrapper<T>> PutAsync<T>(string url, Dictionary<string, string> headers, string jsonData)
        {
            var httpClient = new HttpClient();
            foreach (KeyValuePair<string, string> x in headers)
            {
                httpClient.DefaultRequestHeaders.Add(x.Key, x.Value);
            }

            StringContent stringContent = null;
            if (jsonData.HasValue())
            {
                stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.PutAsync(url, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<T>(response);
                return new HttpResponseWrapper<T>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, response);
            }
        }

        public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
        }

        public async Task<HttpResponseWrapper<object>> DeleteAsync(string url, Dictionary<string, string> headers)
        {
            var httpClient = new HttpClient();
            foreach (KeyValuePair<string, string> x in headers)
            {
                httpClient.DefaultRequestHeaders.Add(x.Key, x.Value);
            }

            var response = await httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
        }
    }
}
