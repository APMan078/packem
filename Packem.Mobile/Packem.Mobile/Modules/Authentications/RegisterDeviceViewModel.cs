using Newtonsoft.Json;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Authentications
{
    public class RegisterDeviceViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;

        private ValidatableObject<string> _registrationToken;

        #endregion

        #region "Properties"

        public ValidatableObject<string> RegistrationToken
        {
            get => _registrationToken;
            set { SetProperty(ref _registrationToken, value); }
        }

        #endregion

        #region "Commands"

        public ICommand RegistrationTokenUnfocusedCommand
            => new Command(RegistrationTokenUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand RegisterCommand
            => new AsyncCommand(Register,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        #endregion

        #region "Functions"

        private void AddValidations()
        {
            _registrationToken = new ValidatableObject<string>();
            _registrationToken.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please enter registration token."
            });
        }

        private bool AreFieldsValid()
        {
            _registrationToken.Validate();

            return _registrationToken.IsValid;
        }

        private void RegistrationTokenUnfocused()
            => _registrationToken.Validate();

        private async Task Register()
        {
            if (!AreFieldsValid())
            {
                return;
            }

            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var result = await _authService
                    .ValidateCustomerDeviceTokenAsync(new Domain.Models.CustomerDeviceTokenValidateTokenModel
                    {
                        DeviceToken = RegistrationToken.Value
                    });

                if (!result.Success)
                {
                    await _dialogService.DisplayAlert("Error", await result.GetBody(), "OK");
                    return;
                }
                else
                {
                    var deviceTokenResult = await _authService
                        .CustomerDeviceTokenInfoAsync(result.Response.DeviceToken);

                    if (!deviceTokenResult.Success)
                    {
                        await _dialogService.DisplayAlert("Error", await deviceTokenResult.GetBody(), "OK");
                        return;
                    }

                    var state = new MobileState
                    {
                        DeviceState = deviceTokenResult.Response
                    };

                    Preferences.Set(Constants.MOBILE_STATE, JsonConvert.SerializeObject(state));
                    await Shell.Current.GoToAsync("//RegisteredDeviceViewModel");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("a", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        #endregion

        public RegisterDeviceViewModel(INavigationService navigationService,
            IAuthService authService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _authService = authService;
            _dialogService = dialogService;

            AddValidations();
        }

        public override Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }

            return Task.CompletedTask;
        }
    }
}
