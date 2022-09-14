using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Authentications
{
    public class LoginViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;
        private readonly IDialogService _dialogService;

        private ValidatableObject<WarehouseViewModel> _warehouse;
        //private string _username;
        //private string _password;

        private ValidatableObject<string> _username;
        private ValidatableObject<string> _password;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<WarehouseViewModel> Warehouses { get; }

        public ValidatableObject<WarehouseViewModel> Warehouse
        {
            get => _warehouse;
            set { SetProperty(ref _warehouse, value); }
        }

        //public string Username
        //{
        //    get => _username;
        //    set { SetProperty(ref _username, value); }
        //}

        //public string Password
        //{
        //    get => _password;
        //    set { SetProperty(ref _password, value); }
        //}

        public ValidatableObject<string> Username
        {
            get => _username;
            set { SetProperty(ref _username, value); }
        }

        public ValidatableObject<string> Password
        {
            get => _password;
            set { SetProperty(ref _password, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand SettingCommand { get => new Command(async () => await Setting()); }

        public ICommand WarehouseChangedCommand
            => new Command(WarehouseChanged,
                canExecute: () => IsNotBusy);

        public ICommand UsernameUnfocusedCommand
            => new Command(UsernameUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand PasswordUnfocusedCommand
            => new Command(PasswordUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand LoginCommand
            => new AsyncCommand(Login,
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

        void WarehouseChanged()
            => _warehouse.Validate();

        void UsernameUnfocused()
            => _username.Validate();

        void PasswordUnfocused()
            => _password.Validate();

        private void AddValidations()
        {
            _warehouse = new ValidatableObject<WarehouseViewModel>();
            _username = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            _warehouse.Validations.Add(new IsNotNullRule<WarehouseViewModel>
            {
                ValidationMessage = "Please select warehouse."
            });

            _username.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please enter username."
            });

            _password.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please enter password."
            });
        }

        private bool AreFieldsValid()
        {
            _warehouse.Validate();
            _username.Validate();
            _password.Validate();

            return _warehouse.IsValid && _username.IsValid && _password.IsValid;
        }

        private async Task Login()
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

                var result = await _authService.AuthenticateUserAsync(new UserLoginModel
                {
                    Username = Username.Value,
                    Password = Password.Value
                });

                if (!result.Success)
                {
                    await _dialogService.DisplayAlert("Error", await result.GetBody(), "OK");
                    return;
                }
                else
                {
                    var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);

                    if (stateJson != null)
                    {
                        var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                        state.Facility = new CustomerFacility
                        {
                            CustomerFacilityId = Warehouse.Value.WarehouseId,
                            Name = Warehouse.Value.Name
                        };

                        state.UserToken = result.Response.Token;

                        var userTokenResult = await _authService
                            .UserTokenInfoAsync(result.Response.Token);

                        if (!userTokenResult.Success)
                        {
                            await _dialogService.DisplayAlert("Error", await userTokenResult.GetBody(), "OK");
                            return;
                        }

                        if (userTokenResult.Response.Role != Domain.Common.Enums.RoleEnum.Operator)
                        {
                            await _dialogService.DisplayAlert("Error", "Only operator user can use this application.", "OK");
                            return;
                        }
                        else
                        {
                            if (userTokenResult.Response.CustomerLocationId != state.DeviceState.CustomerLocationId)
                            {
                                await _dialogService.DisplayAlert("Error", "You don't have access to this customer location.", "OK");
                                return;
                            }

                            state.AppState = userTokenResult.Response;
                            Preferences.Set(Constants.MOBILE_STATE, JsonConvert.SerializeObject(state));
                            Preferences.Set(Constants.IS_USER_LOGGED_IN, true);
                            _navigationService.GoToMainFlow();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private async Task Setting()
        {
            await _dialogService.DisplayAlert("Setting", "Setting", "Ok");
        }

        #endregion

        public LoginViewModel(INavigationService navigationService,
            IAuthService authService,
            ICustomerService customerService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _authService = authService;
            _customerService = customerService;
            _dialogService = dialogService;

            Warehouses = new ObservableRangeCollection<WarehouseViewModel>();
            BindingBase.EnableCollectionSynchronization(Warehouses, null, ObservableCollectionCallback);
            AddValidations();
        }

        void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            // `lock` ensures that only one thread access the collection at a time
            lock (collection)
            {
                accessMethod?.Invoke();
            }
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
                else
                {
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

                        foreach (var x in result.Response.CustomerLocation.CustomerFacilities)
                        {
                            Warehouses.Add(new WarehouseViewModel
                            {
                                WarehouseId = x.CustomerFacilityId,
                                Name = x.Name
                            });
                        }
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
    }
}
