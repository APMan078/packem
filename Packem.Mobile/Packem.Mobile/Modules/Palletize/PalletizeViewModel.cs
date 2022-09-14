using Newtonsoft.Json;
using Packem.Domain.Common.Enums;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Palletize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Palletize
{
    public class PalletizeViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ILicensePlateService _licensePlateService;
        private readonly IUnauthorizedService _unauthorizedService;

        private ValidatableObject<LicensePlateViewModel> _lp;

        private bool _hasRecord;
        private string _noRecordText;
        private bool _addSKUVisible;
        private int _itemCount;
        private int _itemQtyCount;

        #endregion

        #region "Properties"

        public ValidatableObject<LicensePlateViewModel> LP
        {
            get => _lp;
            set { SetProperty(ref _lp, value); }
        }

        public ObservableRangeCollection<PalletizeItemViewModel> Items { get; }

        public bool HasRecord
        {
            get => _hasRecord;
            set => SetProperty(ref _hasRecord, value);
        }

        public string NoRecordText
        {
            get => _noRecordText;
            set => SetProperty(ref _noRecordText, value);
        }

        public bool AddSKUVisible
        {
            get => _addSKUVisible;
            set => SetProperty(ref _addSKUVisible, value);
        }

        public int ItemCount
        {
            get => _itemCount;
            set => SetProperty(ref _itemCount, value);
        }

        public int ItemQtyCount
        {
            get => _itemQtyCount;
            set => SetProperty(ref _itemQtyCount, value);
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand ScanLPCommand
            => new AsyncCommand(ScanLP,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchLPCommand
            => new AsyncCommand(SearchLP,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand LPUnfocusedCommand
            => new Command(LPUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand AddSKUCommand
            => new AsyncCommand(AddSKU,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand CompleteCommand
            => new AsyncCommand(Complete,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand DeleteItemCommand
            => new Command<PalletizeItemViewModel>(DeleteItem,
                canExecute: (e) => IsNotBusy);

        public ICommand ItemTotalQtyUnfocusedCommand
            => new Command<PalletizeItemViewModel>(ItemTotalQtyUnfocused,
                canExecute: (e) => IsNotBusy);

        public ICommand AddLotNoCommand
            => new AsyncCommand<PalletizeItemViewModel>(AddLotNo,
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
            _lp = new ValidatableObject<LicensePlateViewModel>();

            _lp.Validations.Add(new IsNotNullRule<LicensePlateViewModel>
            {
                ValidationMessage = "Please scan or search license plate."
            });
        }

        private bool AreFieldsValid()
        {
            _lp.Validate();

            return _lp.IsValid;
        }

        private async Task ScanLP()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PalletizeLicensePlateScanViewModel>();
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

                await _navigationService.PushAsync<PalletizeLicensePlateLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        void LPUnfocused()
            => _lp.Validate();

        private async Task AddSKU()
        {
            if (!AreFieldsValid())
            {
                return;
            }

            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PalletizeItemLookupViewModel>();
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

            if (Items.Count == 0)
            {
                await _dialogService.DisplayAlert("Warning", "Cannot complete this operation. No SKUs found.", "OK");
                return;
            }

            if (Items.Any(x => x.TotalQty is null))
            {
                await _dialogService.DisplayAlert("Warning", "Cannot complete this operation. There is an empty 'Total Qty'.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);

                if (LP.Value.LicensePlateType == LicensePlateTypeEnum.Unknown)
                {
                    var dto = new LicensePlateUnknownToPalletizedEditModel
                    {
                        LicensePlateId = LP.Value.LicensePlateId
                    };

                    var products = new List<LicensePlateUnknownToPalletizedEditModel.Product>();
                    foreach (var x in Items)
                    {
                        products.Add(new LicensePlateUnknownToPalletizedEditModel.Product
                        {
                            ItemId = x.ItemId,
                            VendorId = x.Vendor?.VendorId,
                            LotId = x.Lot?.LotId,
                            ReferenceNo = x.ReferenceNo,
                            Cases = x.Cases,
                            EaCase = x.EACase,
                            TotalQty = x.TotalQty,
                            TotalWeight = x.TotalWgt
                        });
                    }

                    dto.Products = products;

                    var result = await _licensePlateService
                        .EditLicensePlateUnknownToPalletizedDeviceAsync(state.DeviceState, dto);

                    await _unauthorizedService
                        .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                        {
                            LP.Value = null;
                            Items.Clear();
                            ItemCount = 0;
                            ItemQtyCount = 0;

                            await _dialogService.DisplayAlert("Palletize", "Completed.", "OK");
                        });
                }
                else
                {
                    var result = await _licensePlateService
                        .EditLicensePlateKnownToPalletizedDeviceAsync(state.DeviceState, new LicensePlateKnownToPalletizedEditModel
                        {
                            LicensePlateId = LP.Value.LicensePlateId
                        });

                    await _unauthorizedService
                        .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                        {
                            LP.Value = null;
                            Items.Clear();
                            ItemCount = 0;
                            ItemQtyCount = 0;

                            await _dialogService.DisplayAlert("Palletize", "Completed.", "OK");
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

        private void DeleteItem(PalletizeItemViewModel e)
        {
            try
            {
                IsBusy = true;

                Items.Remove(e);

                ItemCount = 0;
                ItemQtyCount = 0;
                foreach (var x in Items)
                {
                    ItemCount++;

                    if (x.TotalQty != null)
                    {
                        ItemQtyCount += x.TotalQty.Value;
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ItemTotalQtyUnfocused(PalletizeItemViewModel e)
        {
            ItemQtyCount = 0;
            foreach (var x in Items)
            {
                if (x.TotalQty != null)
                {
                    ItemQtyCount += x.TotalQty.Value;
                }
            }
        }

        private async Task AddLotNo(PalletizeItemViewModel e)
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PalletizeAddLotViewModel>($"id={e.ItemId}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnSelectedLP(LicensePlateViewModel e)
        {
            LP.Value = e;
            _lp.Validate();

            Items.Clear();
            ItemCount = 0;
            ItemQtyCount = 0;

            if (LP.Value.LicensePlateType == LicensePlateTypeEnum.Unknown)
            {
                AddSKUVisible = true;
            }
            else
            {
                AddSKUVisible = false;
            }
        }

        private void OnScanLP(LicensePlateViewModel e)
        {
            LP.Value = e;
            _lp.Validate();

            Items.Clear();
            ItemCount = 0;
            ItemQtyCount = 0;

            if (LP.Value.LicensePlateType == LicensePlateTypeEnum.Unknown)
            {
                AddSKUVisible = true;
            }
            else
            {
                AddSKUVisible = false;
            }
        }

        private void OnSelectedLPItem(IEnumerable<PalletizeItemViewModel> e)
        {
            Items.Clear();
            Items.AddRange(e);
            ItemCount = 0;
            ItemQtyCount = 0;

            foreach (var x in Items)
            {
                ItemCount++;

                if (x.TotalQty != null)
                {
                    ItemQtyCount += x.TotalQty.Value;
                }
            }
        }

        private void OnSelectedInventory(ItemVendorViewModel e)
        {
            var pi = new PalletizeItemViewModel();
            pi.ItemId = e.Item.ItemId;
            pi.ItemSKU = e.Item.SKU;
            pi.ItemDescription = e.Item.Description;

            pi.Vendors.Clear();
            pi.Vendors.AddRange(e.Vendors);

            pi.Lots.Clear();
            pi.Lots.AddRange(e.Lots);

            Items.Add(pi);

            ItemCount = Items.Count;
        }

        private void OnAddedItem(LotItemViewModel e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var x in Items.Where(x => x.ItemId == e.ItemId))
                {
                    if (!x.Lots.Any(z => z.LotId == e.LotId))
                    {
                        var lot = new ViewModels.Core.LotViewModel
                        {
                            LotId = e.LotId,
                            LotNo = e.LotNo,
                            ExpirationDate = e.ExpirationDate
                        };

                        x.Lots.Add(lot);
                        x.Lot = x.Lots.SingleOrDefault(z => z.LotId == lot.LotId);
                    }
                }
            });
        }

        #endregion

        public PalletizeViewModel(INavigationService navigationService,
        IDialogService dialogService,
        ILicensePlateService licensePlateService,
        IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _licensePlateService = licensePlateService;
            _unauthorizedService = unauthorizedService;

            Items = new ObservableRangeCollection<PalletizeItemViewModel>();
            BindingBase.EnableCollectionSynchronization(Items, null, ObservableCollectionCallback);

            PalletizeLicensePlateLookupViewModel.SelectedItem += OnSelectedLP;
            PalletizeLicensePlateLookupViewModel.SelectedLPItem += OnSelectedLPItem;
            PalletizeLicensePlateScanViewModel.ScanResult += OnScanLP;
            PalletizeLicensePlateScanViewModel.SelectedLPItem += OnSelectedLPItem;
            PalletizeItemLookupViewModel.SelectedInventory += OnSelectedInventory;
            PalletizeAddLotViewModel.AddedItem += OnAddedItem;

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

                NoRecordText = "ADD LICENSE PLATE";

                Items.CollectionChanged += (sender, args) =>
                {
                    if (Items.Count() > 0)
                        HasRecord = true;
                    else
                    {
                        HasRecord = false;

                        if (!LP.IsValid)
                        {
                            NoRecordText = "ADD LICENSE PLATE";
                        }
                        else
                        {
                            NoRecordText = "PLEASE ADD SKU";
                        }
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