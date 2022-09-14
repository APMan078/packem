using Newtonsoft.Json;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.PurchaseOrders;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.PurchaseOrders
{
    public class PurchaseOrderReceiveViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPutAwayService _putAwayService;
        private readonly IUnauthorizedService _unauthorizedService;

        private ValidatableObject<PurchaseOrderViewModel> _po;
        private ValidatableObject<ReceiveViewModel> _receive;
        private ValidatableObject<int?> _qtyReceived;

        #endregion

        #region "Properties"

        public ValidatableObject<PurchaseOrderViewModel> PO
        {
            get => _po;
            set { SetProperty(ref _po, value); }
        }

        public ValidatableObject<ReceiveViewModel> Receive
        {
            get => _receive;
            set { SetProperty(ref _receive, value); }
        }

        public ValidatableObject<int?> QtyReceived
        {
            get => _qtyReceived;
            set { SetProperty(ref _qtyReceived, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand ClearCommand
            => new Command(Clear,
                canExecute: () => IsNotBusy);

        public ICommand ScanOrderCommand
            => new AsyncCommand(ScanOrder,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchOrderCommand
            => new AsyncCommand(SearchOrder,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand OrderUnfocusedCommand
            => new Command(OrderUnfocused,
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

        public ICommand QtyReceivedUnfocusedCommand
            => new Command(QtyReceivedUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand PrintCommand
            => new AsyncCommand(Print,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand ReceivePrintCommand
            => new AsyncCommand(ReceivePrint,
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

                PO.Value = null;
                Receive.Value = null;
                QtyReceived.Value = null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AddValidations()
        {
            _po = new ValidatableObject<PurchaseOrderViewModel>();
            _receive = new ValidatableObject<ReceiveViewModel>();
            _qtyReceived = new ValidatableObject<int?>();

            _po.Validations.Add(new IsNotNullRule<PurchaseOrderViewModel>
            {
                ValidationMessage = "Please scan or search order."
            });

            _receive.Validations.Add(new IsNotNullRule<ReceiveViewModel>
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
            _po.Validate();
            _receive.Validate();
            _qtyReceived.Validate();

            return _po.IsValid && _receive.IsValid && _qtyReceived.IsValid;
        }

        private async Task ScanOrder()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<OrderScanViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchOrder()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<OrderLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        void OrderUnfocused()
            => _po.Validate();

        private async Task ScanItem()
        {
            try
            {
                IsBusy = true;

                if (PO.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select PO first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<ItemScanViewModel>($"id={PO.Value.PurchaseOrderId}");
                }
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

                if (PO.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select PO first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<ItemLookupViewModel>($"id={PO.Value.PurchaseOrderId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        void ItemUnfocused()
            => _receive.Validate();

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

                //await _navigationService.PushAsync<PurchaseOrderReceivePrintViewModel>();
                await _navigationService.PushAsync<PurchaseOrderReceivePrintWebViewModel>($"sku={Receive.Value.SKU}&desc={Receive.Value.Description}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ReceivePrint()
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

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _putAwayService
                    .CreatePutAwayDeviceAsync(state.DeviceState, new Domain.Models.PutAwayCreateModel
                    {
                        ReceiveId = Receive.Value.ReceiveId,
                        Qty = QtyReceived.Value
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        Receive.Value.Remaining = result.Response.Remaining;
                        QtyReceived.Value = null;
                        await _dialogService.DisplayAlert("PO Receive", "Receiving Success.", "OK");
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

        private void OnSelectedOrder(PurchaseOrderViewModel e)
        {
            PO.Value = e;
            _po.Validate();
            Receive.Value = null;
        }

        private void OnSelectedItem(ReceiveViewModel e)
        {
            Receive.Value = e;
            _receive.Validate();
        }

        private void OnOrderScanResult(PurchaseOrderViewModel e)
        {
            PO.Value = e;
            _po.Validate();
            Receive.Value = null;
        }

        private void OnItemScanResult(ReceiveViewModel e)
        {
            Receive.Value = e;
            _receive.Validate();
        }

        #endregion

        public PurchaseOrderReceiveViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IPutAwayService putAwayService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _putAwayService = putAwayService;
            _unauthorizedService = unauthorizedService;

            OrderLookupViewModel.SelectedOrder += OnSelectedOrder;
            ItemLookupViewModel.SelectedItem += OnSelectedItem;

            OrderScanViewModel.ScanResult += OnOrderScanResult;
            ItemScanViewModel.ScanResult += OnItemScanResult;

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