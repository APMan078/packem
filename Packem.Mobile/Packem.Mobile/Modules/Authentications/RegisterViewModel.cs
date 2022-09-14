using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Authentications
{
    public class RegisterViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        #endregion

        #region "Properties"
        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand SettingCommand
            => new AsyncCommand(Setting,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand RegisterCommand
            => new AsyncCommand(Register,
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

        private async Task Setting()
        {
            try
            {
                IsBusy = true;

                await _dialogService.DisplayAlert("Setting", "Setting", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Register()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<RegisterDeviceViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public RegisterViewModel(INavigationService navigationService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                if (Preferences.ContainsKey(Constants.MOBILE_STATE)
                    && Preferences.Get(Constants.MOBILE_STATE, null) != null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(RegisteredDeviceViewModel)}");
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