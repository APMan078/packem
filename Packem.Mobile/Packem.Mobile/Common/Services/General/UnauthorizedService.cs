using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.Modules.Authentications;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Services.General
{
    public class UnauthorizedService : IUnauthorizedService
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public UnauthorizedService(INavigationService navigationService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        public async Task UnauthorizedWorkflowIfNotAuthenticated<T>(HttpResponseWrapper<T> response, Action authenticated) where T : class
        {
            if (response.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Preferences.Remove(Constants.IS_USER_LOGGED_IN);
                Preferences.Remove(Constants.MOBILE_STATE);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    _navigationService.GoToLoginFlow();
                    await _navigationService.InsertAsRoot<RegisterViewModel>();
                });
            }
            else
            {
                if (!response.Success)
                {
                    await _dialogService.DisplayAlert("Error", await response.GetBody(), "OK");
                }
                else
                {
                    authenticated?.Invoke();
                }
            }
        }
    }
}