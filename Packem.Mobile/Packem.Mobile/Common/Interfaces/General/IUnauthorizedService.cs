using Packem.Mobile.Models;
using System;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.General
{
    public interface IUnauthorizedService
    {
        Task UnauthorizedWorkflowIfNotAuthenticated<T>(HttpResponseWrapper<T> response, Action authenticated) where T : class;
    }
}