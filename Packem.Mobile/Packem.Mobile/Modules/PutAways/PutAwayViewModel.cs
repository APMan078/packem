using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using Packem.Mobile.ViewModels.PutAways;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.PutAways
{
    public class PutAwayViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPutAwayService _putAwayService;
        private readonly IUnauthorizedService _unauthorizedService;

        private ValidatableObject<string> _type;
        private ValidatableObject<PutAwayItemViewModel> _pa;
        private ValidatableObject<ZoneViewModel> _zone;
        private ValidatableObject<string> _bin;
        private ValidatableObject<int?> _qtyPutAway;

        private bool _isEach;
        private bool _hasRecord;

        #endregion

        #region "Properties"

        public ValidatableObject<string> Type
        {
            get => _type;
            set { SetProperty(ref _type, value); }
        }

        public ValidatableObject<PutAwayItemViewModel> PA
        {
            get => _pa;
            set { SetProperty(ref _pa, value); }
        }

        public ValidatableObject<ZoneViewModel> Zone
        {
            get => _zone;
            set { SetProperty(ref _zone, value); }
        }

        public ValidatableObject<string> Bin
        {
            get => _bin;
            set { SetProperty(ref _bin, value); }
        }

        public ValidatableObject<int?> QtyPutAway
        {
            get => _qtyPutAway;
            set { SetProperty(ref _qtyPutAway, value); }
        }

        public bool IsEach
        {
            get => _isEach;
            set { SetProperty(ref _isEach, value); }
        }

        public ObservableRangeCollection<PutAwayLicensePlateViewModel> LPs { get; }

        public bool HasRecord
        {
            get => _hasRecord;
            set { SetProperty(ref _hasRecord, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand ClearCommand
            => new Command(Clear,
                canExecute: () => IsNotBusy);

        public ICommand TypeChangedCommand
            => new Command(TypeChanged,
                canExecute: () => IsNotBusy);

        public ICommand ScanItemCommand
            => new AsyncCommand(ScanItem,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchItemCommand
            => new AsyncCommand(SearchItem,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand ItemUnfocusedCommand
            => new Command(ItemUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ScanLPCommand
            => new AsyncCommand(ScanLP,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchLPCommand
            => new AsyncCommand(SearchLP,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand DeleteLPCommand
            => new Command<PutAwayLicensePlateViewModel>(DeleteLP,
                canExecute: (e) => IsNotBusy);

        public ICommand SearchZoneCommand
            => new AsyncCommand(SearchZone,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand ZoneUnfocusedCommand
            => new Command(ZoneUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ScanBinCommand
            => new AsyncCommand(ScanBin,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchBinCommand
            => new AsyncCommand(SearchBin,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand BinUnfocusedCommand
            => new Command(BinUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand QtyPutAwayUnfocusedCommand
            => new Command(QtyPutAwayUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand PutAwayCommand
            => new AsyncCommand(PutAway,
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

        private void ClearState()
        {
            PA.Value = null;
            Zone.Value = null;
            Bin.Value = null;
            QtyPutAway.Value = null;

            LPs.Clear();
        }

        private void Clear()
        {
            try
            {
                IsBusy = true;

                ClearState();
            }
            finally
            {
                IsBusy = false;
            }
        }

        void TypeChanged()
        {
            _type.Validate();

            if (_type.Value != null && _type.Value == "Each")
            {
                IsEach = true;

                LPs.Clear();
            }
            else
            {
                IsEach = false;

                PA.Value = null;
                QtyPutAway.Value = null;
            }

            Zone.Value = null;
            Bin.Value = null;
        }

        private void AddValidations()
        {
            _type = new ValidatableObject<string>();
            _pa = new ValidatableObject<PutAwayItemViewModel>();
            _zone = new ValidatableObject<ZoneViewModel>();
            _bin = new ValidatableObject<string>();
            _qtyPutAway = new ValidatableObject<int?>();

            _type.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please select a type."
            });

            _pa.Validations.Add(new IsNotNullRule<PutAwayItemViewModel>
            {
                ValidationMessage = "Please scan or search item."
            });

            _zone.Validations.Add(new IsNotNullRule<ZoneViewModel>
            {
                ValidationMessage = "Please scan or search area."
            });

            _bin.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please scan or search location."
            });

            _qtyPutAway.Validations.Add(new IsNotNullRule<int?>
            {
                ValidationMessage = "Please enter qty put away."
            });
        }

        private bool AreFieldsValid()
        {
            _type.Validate();
            _pa.Validate();
            _zone.Validate();
            _bin.Validate();
            _qtyPutAway.Validate();

            return _type.IsValid && _pa.IsValid && _zone.IsValid && _bin.IsValid && _qtyPutAway.IsValid;
        }

        private bool AreFieldsValidLP()
        {
            _type.Validate();
            _zone.Validate();
            _bin.Validate();

            return _type.IsValid && _zone.IsValid && _bin.IsValid;
        }

        private async Task ScanItem()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PutAwayItemScanViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchItem()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PutAwayItemLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        void ItemUnfocused()
            => _pa.Validate();

        private async Task ScanLP()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PutAwayLicensePlateScanViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchLP()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PutAwayLicensePlateLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void DeleteLP(PutAwayLicensePlateViewModel e)
        {
            try
            {
                IsBusy = true;

                LPs.Remove(e);
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

                await _navigationService.PushAsync<PutAwayZoneLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        void ZoneUnfocused()
            => _zone.Validate();

        private async Task ScanBin()
        {
            try
            {
                IsBusy = true;

                if (Zone.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select area first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<PutAwayBinScanViewModel>();
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

                if (PA.Value is null && Type.Value == "Each")
                {
                    await _dialogService.DisplayAlert("Warning", "Please select item first.", "OK");
                }
                else if (LPs.Count == 0 && Type.Value == "Pallet")
                {
                    await _dialogService.DisplayAlert("Warning", "Please add license plate first.", "OK");
                }
                else if (Zone.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select area first.", "OK");
                }
                else
                {
                    if (Type.Value == "Each")
                    {
                        await _navigationService.PushAsync<PutAwayBinLookupViewModel>($"id={Zone.Value.ZoneId}&iid={PA.Value.ItemId}");
                    }
                    else
                    {
                        await _navigationService.PushAsync<PutAwayBinLookupPalletViewModel>($"id={Zone.Value.ZoneId}");
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        void BinUnfocused()
            => _bin.Validate();

        void QtyPutAwayUnfocused()
            => _qtyPutAway.Validate();

        private async Task PutAway()
        {
            if (Type.Value == "Each")
            {
                if (!AreFieldsValid())
                {
                    return;
                }
            }
            else
            {
                if (!AreFieldsValidLP())
                {
                    return;
                }
            }

            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                if (Type.Value == "Each")
                {
                    var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                    var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                    var result = await _putAwayService
                        .CreatePutAwayBinDeviceAsync(state.DeviceState, new PutAwayBinDeviceCreateModel
                        {
                            UserId = state.AppState.UserId,
                            PutAwayId = PA.Value.PutAwayId,
                            BinGetCreate = new BinGetCreateModel
                            {
                                ZoneId = Zone.Value.ZoneId,
                                Name = Bin.Value
                            },
                            Qty = QtyPutAway.Value,
                            PutAwayType = PA.Value.PutAwayType
                        });

                    await _unauthorizedService
                        .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                        {
                            var result2 = await _putAwayService
                                .GetPutAwayDeviceAsync(state.DeviceState, PA.Value.PutAwayId);

                            if (result2.Success)
                            {
                                PA.Value.Remaining = result2.Response.Remaining;
                            }

                            QtyPutAway.Value = null;
                            await _dialogService.DisplayAlert("Put Away", "Success.", "OK");
                        });
                }
                else
                {
                    var pas = new List<PutAwayPalletDeviceCreateModel.PutAway>();
                    foreach (var x in LPs)
                    {
                        pas.Add(new PutAwayPalletDeviceCreateModel.PutAway
                        {
                            PutAwayId = x.PutAwayId
                        });
                    }

                    var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                    var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                    var result = await _putAwayService
                        .CreatePutAwayPalletDeviceAsync(state.DeviceState, new PutAwayPalletDeviceCreateModel
                        {
                            UserId = state.AppState.UserId,
                            CustomerFacilityId = state.Facility.CustomerFacilityId,
                            BinGetCreate = new BinGetCreateModel
                            {
                                ZoneId = Zone.Value.ZoneId,
                                Name = Bin.Value
                            },
                            PutAways = pas
                        });

                    await _unauthorizedService
                        .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                        {
                            ClearState();
                            await _dialogService.DisplayAlert("Put Away", "Success.", "OK");
                        });
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

        private void OnSelectedItem(PutAwayItemViewModel e)
        {
            PA.Value = e;
            _pa.Validate();
        }

        private void OnSelectedZone(ZoneViewModel e)
        {
            Zone.Value = e;
            _zone.Validate();
        }

        private void OnSelectedBin(BinItemQuantityLotViewModel e)
        {
            Bin.Value = e.Name;
            _bin.Validate();
        }

        private void OnScanResult(PutAwayItemViewModel e)
        {
            PA.Value = e;
            _pa.Validate();
        }

        private void OnScanResultBin(string e)
        {
            Bin.Value = e;
            _bin.Validate();
        }

        private void OnScanResultLP(PutAwayLicensePlateViewModel e)
        {
            Type.Value = "Pallet";

            if (!LPs.Any(x => x.PutAwayId == e.PutAwayId))
            {
                LPs.Add(e);
            }
        }

        private void OnSelectedItemLP(PutAwayLicensePlateViewModel e)
        {
            Type.Value = "Pallet";

            if (!LPs.Any(x => x.PutAwayId == e.PutAwayId))
            {
                LPs.Add(e);
            }
        }

        private void OnSelectedBinPallet(BinOptionalPalletViewModel e)
        {
            Bin.Value = e.Name;
            _bin.Validate();
        }

        #endregion

        public PutAwayViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IPutAwayService putAwayService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _putAwayService = putAwayService;
            _unauthorizedService = unauthorizedService;

            LPs = new ObservableRangeCollection<PutAwayLicensePlateViewModel>();
            BindingBase.EnableCollectionSynchronization(LPs, null, ObservableCollectionCallback);

            PutAwayItemLookupViewModel.SelectedItem += OnSelectedItem;
            PutAwayZoneLookupViewModel.SelectedItem += OnSelectedZone;
            PutAwayBinLookupViewModel.SelectedItem += OnSelectedBin;
            PutAwayItemScanViewModel.ScanResult += OnScanResult;
            PutAwayBinScanViewModel.ScanResult += OnScanResultBin;

            PutAwayLicensePlateScanViewModel.ScanResult += OnScanResultLP;
            PutAwayLicensePlateLookupViewModel.SelectedItem += OnSelectedItemLP;
            PutAwayBinLookupPalletViewModel.SelectedItem += OnSelectedBinPallet;

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

        public override Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                Type.Value = "Each";

                LPs.CollectionChanged += (sender, args) =>
                {
                    if (LPs.Count() > 0)
                    {
                        HasRecord = true;

                        foreach (var x in LPs)
                        {
                            if (LPs.IndexOf(x) == LPs.Count - 1) // last row
                            {
                                x.ShowSeparator = false;
                            }
                            else
                            {
                                x.ShowSeparator = true;
                            }
                        }
                    }
                    else
                    {
                        HasRecord = false;
                    }
                };
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