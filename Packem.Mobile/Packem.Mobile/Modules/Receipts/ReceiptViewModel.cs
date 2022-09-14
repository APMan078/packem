using Newtonsoft.Json;
using Packem.Domain.Entities;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.Modules.Picking;
using Packem.Mobile.Modules.PurchaseOrders;
using Packem.Mobile.ViewModels.Core;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Packem.Mobile.Modules.Receipts
{
    public class ReceiptViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IReceiptService _receiptService;
        private readonly IUnauthorizedService _unauthorizedService;

        private ValidatableObject<ItemViewModel> _item;
        private ValidatableObject<int?> _qtyReceived;
        private LotViewModel _lot;
        private bool hasLot;

        #endregion

        #region "Properties"

        public ValidatableObject<ItemViewModel> Item
        {
            get => _item;
            set { SetProperty(ref _item, value); }
        }

        public ValidatableObject<int?> QtyReceived
        {
            get => _qtyReceived;
            set { SetProperty(ref _qtyReceived, value); }
        }

        public LotViewModel Lot
        {
            get => _lot;
            set { SetProperty(ref _lot, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand ClearCommand
            => new Command(Clear,
                canExecute: () => IsNotBusy);

        public ICommand ScanItemCommand
            => new AsyncCommand(ScanItem,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchItemCommand
            => new AsyncCommand(SearchItem,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand AddLotCommand
            => new AsyncCommand(AddLot,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchLotCommand
            => new AsyncCommand(SearchLot,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand ItemUnfocusedCommand
            => new Command(ItemUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand QtyReceivedUnfocusedCommand
            => new Command(QtyReceivedUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand PrintCommand
            => new AsyncCommand(Print,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand QueuePACommand
            => new AsyncCommand(QueuePA,
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

        private void Clear()
        {
            try
            {
                IsBusy = true;

                Item.Value = null;
                QtyReceived.Value = null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AddValidations()
        {
            _item = new ValidatableObject<ItemViewModel>();
            _qtyReceived = new ValidatableObject<int?>();

            _item.Validations.Add(new IsNotNullRule<ItemViewModel>
            {
                ValidationMessage = "Please scan or search item."
            });

            _qtyReceived.Validations.Add(new IsNotNullRule<int?>
            {
                ValidationMessage = "Please enter qty received."
            });
        }

        private bool AreFieldsValid()
        {
            _item.Validate();
            _qtyReceived.Validate();

            return _item.IsValid && _qtyReceived.IsValid;
        }

        private async Task ScanItem()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<ReceiptItemScanViewModel>();
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

                await _navigationService.PushAsync<ReceiptItemLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddLot()
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
                    await _navigationService.PushAsync<ReceiptAddLotViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchLot()
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
                    await _navigationService.PushAsync<ReceiptLotLookupViewModel>($"id={Item.Value.ItemId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        void ItemUnfocused()
            => _item.Validate();

        void QtyReceivedUnfocused()
            => _qtyReceived.Validate();

        private async Task Print()
        {
            if (!AreFieldsValid())
            {
                return;
            }

            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<ReceiptPrintWebViewModel>($"sku={Item.Value.SKU}&desc={Item.Value.Description}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task QueuePA()
        {
            if (!AreFieldsValid())
            {
                return;
            }

            try
            {
                IsBusy = true;
                CurrentState = LayoutState.Loading;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                LotGetCreateModel lotGetCreate = null;
                if (hasLot)
                {
                    lotGetCreate = new LotGetCreateModel();

                    if (Lot.LotId == 0) // create
                    {
                        lotGetCreate.LotId = null;
                        lotGetCreate.LotNo = Lot.LotNo;
                        lotGetCreate.ExpirationDate = Lot.ExpirationDate;
                    }
                    else
                    {
                        lotGetCreate.LotId = Lot.LotId;
                    }
                }

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _receiptService
                    .CreateReceiptDeviceAsync(state.DeviceState, new ReceiptDeviceCreateModel
                    {
                        ItemId = Item.Value.ItemId,
                        Qty = QtyReceived.Value,
                        NewLotGetCreate = lotGetCreate
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        Item.Value = null;
                        Lot = null;
                        QtyReceived.Value = null;
                        hasLot = false;

                        await _dialogService.DisplayAlert("Receipt", "Receipt Success.", "OK");
                    });
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
                CurrentState = LayoutState.Success;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private void OnScanResult(ItemViewModel e)
        {
            Item.Value = e;
            _item.Validate();
        }

        private void OnSelectedItem(ItemViewModel e)
        {
            Item.Value = e;
            _item.Validate();
        }

        private void OnAddedItem(LotViewModel e)
        {
            Lot = e;
            hasLot = true;
        }

        private void OnSelectedItem(LotViewModel e)
        {
            Lot = e;
            hasLot = true;
        }

        #endregion

        public ReceiptViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IReceiptService receiptService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _receiptService = receiptService;
            _unauthorizedService = unauthorizedService;

            ReceiptItemLookupViewModel.SelectedItem += OnSelectedItem;
            ReceiptItemScanViewModel.ScanResult += OnScanResult;

            ReceiptAddLotViewModel.AddedItem += OnAddedItem;
            ReceiptLotLookupViewModel.SelectedItem += OnSelectedItem;

            AddValidations();
        }

        public override Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                CurrentState = LayoutState.Loading;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();
            }
            finally
            {
                IsBusy = false;
                CurrentState = LayoutState.Success;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }

            return Task.CompletedTask;
        }
    }
}