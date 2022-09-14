using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Transfers
{
    public class TransferManualViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ITransferService _transferService;
        private readonly IUnauthorizedService _unauthorizedService;

        private ValidatableObject<ItemViewModel> _item;
        private ValidatableObject<ZoneViewModel> _zoneFrom;
        private ValidatableObject<BinItemQuantityViewModel> _binFrom;
        private ValidatableObject<ZoneViewModel> _zoneTo;
        private ValidatableObject<string> _binTo;
        private ValidatableObject<int?> _qtyTransfered;
        private BinItemQuantityLotViewModel _binToProp;

        #endregion

        #region "Properties"

        public ValidatableObject<ItemViewModel> Item
        {
            get => _item;
            set { SetProperty(ref _item, value); }
        }

        public ValidatableObject<ZoneViewModel> ZoneFrom
        {
            get => _zoneFrom;
            set { SetProperty(ref _zoneFrom, value); }
        }

        public ValidatableObject<BinItemQuantityViewModel> BinFrom
        {
            get => _binFrom;
            set { SetProperty(ref _binFrom, value); }
        }

        public ValidatableObject<ZoneViewModel> ZoneTo
        {
            get => _zoneTo;
            set { SetProperty(ref _zoneTo, value); }
        }

        public ValidatableObject<string> BinTo
        {
            get => _binTo;
            set { SetProperty(ref _binTo, value); }
        }

        public ValidatableObject<int?> QtyTransfered
        {
            get => _qtyTransfered;
            set { SetProperty(ref _qtyTransfered, value); }
        }

        public BinItemQuantityLotViewModel BinToProp
        {
            get => _binToProp;
            set { SetProperty(ref _binToProp, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand ScanItemCommand
            => new AsyncCommand(ScanItem,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchCommand
            => new AsyncCommand(Search,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand SearchZoneCommand
            => new AsyncCommand(SearchZone,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand ScanBinCommand
            => new AsyncCommand(ScanBin,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchBinCommand
            => new AsyncCommand(SearchBin,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand SearchZoneToCommand
            => new AsyncCommand(SearchZoneTo,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand ScanBinToCommand
            => new AsyncCommand(ScanBinTo,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchBinToCommand
            => new AsyncCommand(SearchBinTo,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand CompleteCommand
            => new AsyncCommand(Complete,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand ItemUnfocusedCommand
            => new Command(ItemUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ZoneUnfocusedCommand
            => new Command(ZoneUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand BinUnfocusedCommand
            => new Command(BinUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ZoneToUnfocusedCommand
            => new Command(ZoneToUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand BinToUnfocusedCommand
            => new Command(BinToUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand QtyTransferedUnfocusedCommand
            => new Command(QtyTransferedUnfocused,
                canExecute: () => IsNotBusy);

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
            _item = new ValidatableObject<ItemViewModel>();
            _zoneFrom = new ValidatableObject<ZoneViewModel>();
            _binFrom = new ValidatableObject<BinItemQuantityViewModel>();
            _zoneTo = new ValidatableObject<ZoneViewModel>();
            _binTo = new ValidatableObject<string>();
            _qtyTransfered = new ValidatableObject<int?>();

            _item.Validations.Add(new IsNotNullRule<ItemViewModel>
            {
                ValidationMessage = "Please scan or search item."
            });

            _zoneFrom.Validations.Add(new IsNotNullRule<ZoneViewModel>
            {
                ValidationMessage = "Please scan or search area."
            });

            _binFrom.Validations.Add(new IsNotNullRule<BinItemQuantityViewModel>
            {
                ValidationMessage = "Please scan or search location."
            });

            _zoneTo.Validations.Add(new IsNotNullRule<ZoneViewModel>
            {
                ValidationMessage = "Please scan or search area."
            });

            _binTo.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please scan or search location."
            });

            _qtyTransfered.Validations.Add(new IsNotNullRule<int?>
            {
                ValidationMessage = "Please enter qty transfered."
            });
        }

        private bool AreFieldsValid()
        {
            _item.Validate();
            _zoneFrom.Validate();
            _binFrom.Validate();
            _zoneTo.Validate();
            _binTo.Validate();
            _qtyTransfered.Validate();

            return _item.IsValid && _zoneFrom.IsValid && _binFrom.IsValid
                && _zoneTo.IsValid && _binTo.IsValid
                && _qtyTransfered.IsValid;
        }

        private async Task ScanItem()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<TransferManualItemScanViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Search()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<TransferManualItemLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchZone()
        {
            try
            {
                IsBusy = true;

                if (Item.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select item first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<TransferManualItemZoneFromLookupViewModel>($"id={Item.Value.ItemId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ScanBin()
        {
            try
            {
                IsBusy = true;

                if (ZoneFrom.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select area first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<TransferManualItemBinFromScanViewModel>($"zid={ZoneFrom.Value.ZoneId}&iid={Item.Value.ItemId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchBin()
        {
            try
            {
                IsBusy = true;

                if (ZoneFrom.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select area first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<TransferManualItemBinFromLookupViewModel>($"zid={ZoneFrom.Value.ZoneId}&iid={Item.Value.ItemId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchZoneTo()
        {
            try
            {
                IsBusy = true;

                if (Item.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select item first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<TransferManualItemZoneToLookupViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ScanBinTo()
        {
            try
            {
                IsBusy = true;

                if(ZoneTo.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select area first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<TransferManualItemBinToScanViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchBinTo()
        {
            try
            {
                IsBusy = true;

                if (ZoneTo.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select area first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<TransferManualItemBinToLookupViewModel>($"id={ZoneTo.Value.ZoneId}&iid={Item.Value.ItemId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Complete()
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

                var result = await _transferService
                    .CreateTransferManualDeviceAsync(state.DeviceState, new TransferManualCreateModel
                    {
                        UserId = state.AppState.UserId,
                        ItemId = Item.Value.ItemId,
                        ItemFacilityId = state.Facility.CustomerFacilityId,
                        ItemZoneId = ZoneFrom.Value.ZoneId,
                        ItemBinId = BinFrom.Value.BinId,
                        NewBinGetCreate = new BinGetCreateModel
                        {
                            ZoneId = ZoneTo.Value.ZoneId,
                            Name = BinTo.Value
                        },
                        QtyToTransfer = QtyTransfered.Value
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        await _dialogService.DisplayAlert("Transfer", "Transfer Success.", "OK");
                        await _navigationService.PopAsync();
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private void OnSelectedInventory(ItemViewModel e)
        {
            Item.Value = e;
            _item.Validate();
            ZoneFrom.Value = null;
            BinFrom.Value = null;
        }

        private void OnScanResult(ItemViewModel e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Item.Value = e;
                _item.Validate();
                ZoneFrom.Value = null;
                BinFrom.Value = null;
            });
        }

        private void OnSelectedZone(ZoneItemQuantityViewModel e)
        {
            ZoneFrom.Value = new ZoneViewModel
            {
                ZoneId = e.ZoneId,
                Name = e.Name
            };
            _zoneFrom.Validate();

            BinFrom.Value = null;
        }

        private void OnSelectedBin(BinItemQuantityViewModel e)
        {
            BinFrom.Value = e;
            _binFrom.Validate();
        }

        private void OnSelectedZoneTo(ZoneViewModel e)
        {
            ZoneTo.Value = e;
            _zoneTo.Validate();
            BinTo.Value = null;
            BinToProp = null;
        }

        private void OnSelectedBinTo(BinItemQuantityLotViewModel e)
        {
            BinTo.Value = e.Name;
            BinToProp = e;
            _binTo.Validate();
        }

        private void OnScanResult(BinItemQuantityViewModel e)
        {
            BinFrom.Value = e;
            _binFrom.Validate();
        }

        private void ScanResultBinTo(string e)
        {
            BinTo.Value = e;
            //BinToProp = e;
            _binTo.Validate();
        }

        void ItemUnfocused()
            => _item.Validate();

        void ZoneUnfocused()
            => _zoneFrom.Validate();

        void BinUnfocused()
            => _binFrom.Validate();

        void ZoneToUnfocused()
            => _zoneTo.Validate();

        void BinToUnfocused()
            => _binTo.Validate();

        void QtyTransferedUnfocused()
            => _qtyTransfered.Validate();

        #endregion

        public TransferManualViewModel(INavigationService navigationService,
            IDialogService dialogService,
            ITransferService transferService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _transferService = transferService;
            _unauthorizedService = unauthorizedService;

            TransferManualItemLookupViewModel.SelectedInventory += OnSelectedInventory;
            TransferManualItemScanViewModel.ScanResult += OnScanResult;

            TransferManualItemZoneFromLookupViewModel.SelectedZone += OnSelectedZone;
            TransferManualItemBinFromLookupViewModel.SelectedBin += OnSelectedBin;

            TransferManualItemZoneToLookupViewModel.SelectedZone += OnSelectedZoneTo;
            TransferManualItemBinToLookupViewModel.SelectedBin += OnSelectedBinTo;

            TransferManualItemBinFromScanViewModel.ScanResult += OnScanResult;
            TransferManualItemBinToScanViewModel.ScanResult += ScanResultBinTo;

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