using Newtonsoft.Json;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Authentications
{
    public class RegisteredDeviceViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;
        private readonly IDialogService _dialogService;

        private string _clientName;

        #endregion

        #region "Properties"

        public string ClientName
        {
            get => _clientName;
            set { SetProperty(ref _clientName, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand LoginCommand
            => new AsyncCommand(Login,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand UnregisterDeviceCommand
            => new AsyncCommand(UnregisterDevice,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        #endregion

        #region "Functions"

        private async Task Appearing()
        {
            if (!Initialized)
            {
                await InitializeAsync();
                Initialized = true;
            }
        }

        private async Task Login()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<LoginViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UnregisterDevice()
        {
            try
            {
                IsBusy = true;

                var result = await _dialogService
                    .DisplayActionSheet("Unregister Device", null, "Yes", "No");

                if (result == "Yes")
                {
                    ActivityIndicatorViewModel
                        = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                    var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                    if (stateJson != null)
                    {
                        var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                        var resultDeactivate = await _authService
                            .DeactivateCustomerDeviceTokenAsync(new Domain.Models.CustomerDeviceTokenValidateTokenModel
                            {
                                DeviceToken = state.DeviceState.DeviceToken
                            }, state.DeviceState.DeviceToken);

                        Preferences.Remove(Constants.MOBILE_STATE);

                        if (!resultDeactivate.Success)
                        {
                            await _dialogService.DisplayAlert("Error", await resultDeactivate.GetBody(), "OK");
                        }

                        await _navigationService.InsertAsRoot<RegisterViewModel>();
                    }
                    else
                    {
                        await _navigationService.InsertAsRoot<RegisterViewModel>();
                    }
                }
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        #endregion

        public RegisteredDeviceViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IAuthService authService,
            ICustomerService customerService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authService = authService;
            _customerService = customerService;
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                if (Preferences.ContainsKey(Constants.IS_USER_LOGGED_IN)
                    && Preferences.Get(Constants.IS_USER_LOGGED_IN, false) == true)
                {
                    // go to main page
                    _navigationService.GoToMainFlow();
                }

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);

                if (stateJson != null)
                {
                    var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                    var result = await _customerService
                        .GetCurrentCustomerForDeviceAsync(state.DeviceState);

                    if (!result.Success)
                    {
                        await _dialogService.DisplayAlert("Error", await result.GetBody(), "OK");
                        await _navigationService.InsertAsRoot<RegisterViewModel>();
                    }

                    ClientName = result.Response.Name;
                }
                else
                {
                    await _navigationService.InsertAsRoot<RegisterViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }
    }
}
