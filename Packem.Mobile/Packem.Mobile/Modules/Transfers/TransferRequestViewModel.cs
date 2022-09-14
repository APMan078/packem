using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using Packem.Mobile.ViewModels.Transfers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Transfers
{
    public class TransferRequestViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ITransferService _transferService;
        private readonly IBinService _binService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<IEnumerable<TransferLookupDeviceGetModel>> _reloadViewEventManager =
            new WeakEventManager<IEnumerable<TransferLookupDeviceGetModel>>();

        private TransferItemViewModel _transfer;
        private ValidatableObject<ZoneViewModel> _zoneTo;
        private ValidatableObject<string> _binTo;
        private ValidatableObject<int?> _qtyTransfered;

        private bool _zoneToEdit;
        private bool _binToEdit;
        private BinItemQuantityLotViewModel _binToProp;

        #endregion

        #region "Properties"

        public TransferItemViewModel Transfer
        {
            get => _transfer;
            set { SetProperty(ref _transfer, value); }
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

        public bool ZoneToEdit
        {
            get => _zoneToEdit;
            set { SetProperty(ref _zoneToEdit, value); }
        }

        public bool BinToEdit
        {
            get => _binToEdit;
            set { SetProperty(ref _binToEdit, value); }
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

        public ICommand SearchZoneToCommand
            => new AsyncCommand(SearchZoneTo,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand ZoneToUnfocusedCommand
            => new Command(ZoneToUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ScanBinToCommand
            => new AsyncCommand(ScanBinTo,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchBinToCommand
            => new AsyncCommand(SearchBinTo,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand BinToUnfocusedCommand
            => new Command(BinToUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand QtyTransferedUnfocusedCommand
            => new Command(QtyTransferedUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand CompleteCommand
            => new AsyncCommand(Complete,
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
            _zoneTo = new ValidatableObject<ZoneViewModel>();
            _binTo = new ValidatableObject<string>();
            _qtyTransfered = new ValidatableObject<int?>();

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
            _zoneTo.Validate();
            _binTo.Validate();
            _qtyTransfered.Validate();

            return _zoneTo.IsValid && _binTo.IsValid
                && _qtyTransfered.IsValid;
        }

        private void OnSelectedItem(TransferItemViewModel e)
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                Transfer = e;

                if (Transfer.NewZoneId is null)
                {
                    ZoneToEdit = true;
                    ZoneTo.Value = null;
                }
                else
                {
                    ZoneToEdit = false;

                    ZoneTo.Value = new ZoneViewModel
                    {
                        ZoneId = Transfer.NewZoneId.Value,
                        Name = Transfer.NewZone
                    };
                }

                if (Transfer.NewBinId is null)
                {
                    BinToEdit = true;
                    BinTo.Value = null;
                    BinToProp = null;
                }
                else
                {
                    BinToEdit = false;

                    BinTo.Value = Transfer.NewBin;
                    BinToProp = new BinItemQuantityLotViewModel
                    {
                        LotNo = Transfer.LotNo,
                        ExpirationDate = Transfer.ExpirationDate
                    };
                }
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private async Task SearchZoneTo()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<TransferRequestZoneToLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        void ZoneToUnfocused()
            => _zoneTo.Validate();

        private async Task ScanBinTo()
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
                    await _navigationService.PushAsync<TransferRequestBinToScanViewModel>();
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
                    await _navigationService.PushAsync<TransferRequestBinToLookupViewModel>($"id={ZoneTo.Value.ZoneId}&iid={Transfer.ItemId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        void BinToUnfocused()
            => _binTo.Validate();

        void QtyTransferedUnfocused()
            => _qtyTransfered.Validate();

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
                    .CreateTransferRequestDeviceAsync(state.DeviceState, new TransferRequestCreateModel
                    {
                        UserId = state.AppState.UserId,
                        TransferId = Transfer.TransferId,
                        NewBinGetCreate = new BinGetCreateModel
                        {
                            ZoneId = ZoneTo.Value.ZoneId,
                            Name = BinTo.Value
                        },
                        QtyTransfered = QtyTransfered.Value
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        Transfer.Remaining = result.Response.Remaining;

                        if (Transfer.Remaining == 0)
                        {
                            var result2 = await _transferService
                                .GetTransferLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                            if (result2.Success)
                            {
                                _reloadViewEventManager.RaiseEvent(result2.Response, nameof(ReloadView));
                            }

                            await _dialogService.DisplayAlert("Transfer", "Transfer Success.", "OK");
                            await _navigationService.PopAsync();
                        }
                        else
                        {
                            var result2 = await _binService
                                .GetBinItemQuantityDeviceAsync(state.DeviceState, Transfer.ItemId, Transfer.CurrentBinId);

                            if (result2.Success)
                            {
                                Transfer.CurrentBinQty = result2.Response.Qty;
                            }

                            if (ZoneToEdit)
                            {
                                ZoneTo.Value = null;
                            }

                            if (BinToEdit)
                            {
                                BinTo.Value = null;
                                BinToProp = null;
                            }

                            QtyTransfered.Value = null;
                            await _dialogService.DisplayAlert("Transfer", "Transfer Success.", "OK");
                        }
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        public static event Action<IEnumerable<TransferLookupDeviceGetModel>> ReloadView
        {
            add => _reloadViewEventManager.AddEventHandler(value);
            remove => _reloadViewEventManager.RemoveEventHandler(value);
        }

        private void OnSelectedZone(ZoneViewModel e)
        {
            ZoneTo.Value = e;
            _zoneTo.Validate();
            BinTo.Value = null;
            BinToProp = null;
        }

        private void OnSelectedBin(BinItemQuantityLotViewModel e)
        {
            BinTo.Value = e.Name;
            BinToProp = e;
            _binTo.Validate();
        }

        private void OnScanResult(string e)
        {
            BinTo.Value = e;
            //BinToProp = e;
            _binTo.Validate();
        }

        #endregion

        public TransferRequestViewModel(INavigationService navigationService,
            IDialogService dialogService,
            ITransferService transferService,
            IBinService binService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _transferService = transferService;
            _binService = binService;
            _unauthorizedService = unauthorizedService;

            TransferViewModel.SelectedItem += OnSelectedItem;
            TransferRequestZoneToLookupViewModel.SelectedZone += OnSelectedZone;
            TransferRequestBinToLookupViewModel.SelectedBin += OnSelectedBin;
            TransferRequestBinToScanViewModel.ScanResult += OnScanResult;

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