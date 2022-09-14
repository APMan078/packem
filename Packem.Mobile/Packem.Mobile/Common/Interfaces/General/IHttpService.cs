using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.General
{
    public interface IHttpService
    {
        Task<HttpResponseWrapper<T>> GetAsync<T>(string uri);
        Task<HttpResponseWrapper<T>> GetAsync<T>(string uri, Dictionary<string, string> headers);
        Task<HttpResponseWrapper<T>> PostAsync<T>(string uri, string jsonData);
        Task<HttpResponseWrapper<T>> PostAsync<T>(string uri, Dictionary<string, string> headers, string jsonData);
        Task<HttpResponseWrapper<T>> PutAsync<T>(string uri, string jsonData);
        Task<HttpResponseWrapper<T>> PutAsync<T>(string uri, Dictionary<string, string> headers, string jsonData);
        Task<HttpResponseWrapper<object>> DeleteAsync(string uri);
        Task<HttpResponseWrapper<object>> DeleteAsync(string uri, Dictionary<string, string> headers);
    }
}
