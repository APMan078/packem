using System.Net.Http;
using System.Threading.Tasks;

namespace Packem.Mobile.Models
{
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(T response, bool success, HttpResponseMessage httpResponseMessage)
        {
            Success = success;
            Response = response;
            HttpResponseMessage = httpResponseMessage;
        }

        public bool Success { get; private set; }
        public T Response { get; private set; }
        public HttpResponseMessage HttpResponseMessage { get; private set; }

        public async Task<string> GetBody()
        {
            return await HttpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}