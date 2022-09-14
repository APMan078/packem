using Newtonsoft.Json;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Palletize;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Palletize
{
    [QueryProperty("ItemId", "id")]
    public class PalletizeAddLotViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ILotService _lotService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<LotItemViewModel> _selectedItemEventManager =
            new WeakEventManager<LotItemViewModel>();

        private int _itemId;

        private ValidatableObject<string> _lotNo;
        private ValidatableObject<string> _expirationDate;
        private bool _pickerOpen;

        #endregion

        #region "Properties"

        public int ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }

        public ValidatableObject<string> LotNo
        {
            get => _lotNo;
            set { SetProperty(ref _lotNo, value); }
        }

        public ValidatableObject<string> ExpirationDate
        {
            get => _expirationDate;
            set { SetProperty(ref _expirationDate, value); }
        }

        public bool PickerOpen
        {
            get => _pickerOpen;
            set { SetProperty(ref _pickerOpen, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand LotNoUnfocusedCommand
            => new Command(LotNoUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ExpirationDateUnfocusedCommand
            => new Command(ExpirationDateUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand SearchExpirationDateCommand
            => new Command(SearchExpirationDate,
                canExecute: () => IsNotBusy);

        public ICommand ChangeExpirationDateCommand
            => new Command<object>(ChangeExpirationDate,
                canExecute: (e) => IsNotBusy);

        public ICommand CancelLotCommand
            => new AsyncCommand(CancelLot,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand AddLotCommand
            => new AsyncCommand(AddLot,
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

        private void AddValidations()
        {
            _lotNo = new ValidatableObject<string>();
            _expirationDate = new ValidatableObject<string>();

            _lotNo.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please enter a lot no."
            });

            _expirationDate.Validations.Add(new DateTimeRule<string>
            {
                ValidationMessage = "Please enter expiration date (m/d/y)."
            });
        }

        private bool AreFieldsValid()
        {
            _lotNo.Validate();
            _expirationDate.Validate();

            return _lotNo.IsValid && _expirationDate.IsValid;
        }

        void LotNoUnfocused()
            => _lotNo.Validate();

        void ExpirationDateUnfocused()
            => _expirationDate.Validate();

        private void SearchExpirationDate()
        {
            try
            {
                IsBusy = true;

                PickerOpen = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ChangeExpirationDate(object e)
        {
            try
            {
                IsBusy = true;

                var datePicker = e as Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;
                var date = GetSelectedItems(datePicker.NewValue as ICollection);
                var dateString = date.Remove(date.Length - 1, 1);

                ExpirationDate.Value = dateString;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string GetSelectedItems(ICollection collection)
        {
            string dates = string.Empty;
            int i = 0;
            foreach (var item in collection)
            {
                dates += item;
                dates += "/";
                i++;
            }
            return dates;
        }

        private async Task CancelLot()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PopAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddLot()
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

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _lotService
                    .CreateLotDeviceAsync(state.DeviceState, new Domain.Models.LotCreateModel
                    {
                        CustomerId = state.DeviceState.CustomerId,
                        CustomerLocationId = state.DeviceState.CustomerLocationId,
                        ItemId = ItemId,
                        LotNo = LotNo.Value,
                        ExpirationDate = Convert.ToDateTime(ExpirationDate.Value)
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        _selectedItemEventManager.RaiseEvent(new LotItemViewModel
                        {
                            LotId = result.Response.LotId,
                            ItemId = ItemId,
                            LotNo = LotNo.Value,
                            ExpirationDate = Convert.ToDateTime(ExpirationDate.Value)
                        }, nameof(AddedItem));

                        await _navigationService.PopAsync();
                    });
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

        public static event Action<LotItemViewModel> AddedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PalletizeAddLotViewModel(INavigationService navigationService,
            IDialogService dialogService,
            ILotService lotService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _lotService = lotService;
            _unauthorizedService = unauthorizedService;

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