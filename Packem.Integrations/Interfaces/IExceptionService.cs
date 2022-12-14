using System;
using System.Threading.Tasks;

namespace Packem.Integrations.Interfaces
{
    public interface IExceptionService
    {
        Task<Exception> HandleExceptionAsync(Exception ex);
        Exception GetException(Exception ex);
        Task LogMessageAsync(string message, string stack);
    }
}
