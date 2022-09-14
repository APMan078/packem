using Packem.Mobile.Application;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Services.General
{
    public class ShellRoutingService : INavigationService
    {
        public void GoToMainFlow()
        {
            Xamarin.Forms.Application.Current.MainPage = new AppShell();
        }

        public void GoToLoginFlow()
        {
            Xamarin.Forms.Application.Current.MainPage = new AuthenticationShell();
        }

        public Task PopAsync()
        {
            return Shell.Current.Navigation.PopAsync();
        }

        public Task GoBackAsync()
        {
            return Shell.Current.GoToAsync("..");
        }

        public Task InsertAsRoot<TViewModel>(string parameters = null) where TViewModel : BaseViewModel
        {
            return GoToAsync<TViewModel>("//", parameters);
        }

        public Task PushAsync<TViewModel>(string parameters = null) where TViewModel : BaseViewModel
        {
            return GoToAsync<TViewModel>("", parameters);
        }

        private Task GoToAsync<TViewModel>(string routePrefix, string parameters) where TViewModel : BaseViewModel
        {
            var route = routePrefix + typeof(TViewModel).Name;
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                route += $"?{parameters}";
            }
            return Shell.Current.GoToAsync(route);
        }
    }
}
