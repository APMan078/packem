using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using Packem.Mobile.ViewModels.Picking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Picking
{
    [QueryProperty("SaleOrderId", "id")]
    [QueryProperty("SaleOrderNo", "sNo")]
    public class PickingViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IOrderLineService _orderLineService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<IEnumerable<OrderLinePickLookupGetModel>> _reloadViewEventManager =
            new WeakEventManager<IEnumerable<OrderLinePickLookupGetModel>>();
        private static readonly WeakEventManager<OrderLineGetModel> _reloadViewEventManager2 =
            new WeakEventManager<OrderLineGetModel>();

        private int saleOrderId;
        private string saleOrderNo;
        private string title;
        private OrderLinePickLookupViewModel orderLine;
        private ValidatableObject<ZoneViewModel> _zone;
        private ValidatableObject<BinItemQuantityViewModel> _bin;
        private ValidatableObject<int?> _pickQty;

        #endregion

        #region "Properties"

        public int SaleOrderId
        {
            get => saleOrderId;
            set => SetProperty(ref saleOrderId, value);

        }

        public string SaleOrderNo
        {
            get => saleOrderNo;
            set => SetProperty(ref saleOrderNo, value);

        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);

        }

        public OrderLinePickLookupViewModel OrderLine
        {
            get => orderLine;
            set => SetProperty(ref orderLine, value);

        }

        public ValidatableObject<ZoneViewModel> Zone
        {
            get => _zone;
            set { SetProperty(ref _zone, value); }
        }

        public ValidatableObject<BinItemQuantityViewModel> Bin
        {
            get => _bin;
            set { SetProperty(ref _bin, value); }
        }

        public ValidatableObject<int?> PickQty
        {
            get => _pickQty;
            set { SetProperty(ref _pickQty, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand SearchZoneCommand
            => new AsyncCommand(SearchZone,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand ZoneUnfocusedCommand
            => new Command(ZoneUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ScanBinCommand
            => new AsyncCommand(ScanBin,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchBinCommand
            => new AsyncCommand(SearchBin,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand BinUnfocusedCommand
           => new Command(BinUnfocused,
               canExecute: () => IsNotBusy);

        public ICommand PickQtyUnfocusedCommand
           => new Command(PickQtyUnfocused,
               canExecute: () => IsNotBusy);

        public ICommand SaveCommand
            => new AsyncCommand(Save,
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
            _zone = new ValidatableObject<ZoneViewModel>();
            _bin = new ValidatableObject<BinItemQuantityViewModel>();
            _pickQty = new ValidatableObject<int?>();

            _zone.Validations.Add(new IsNotNullRule<ZoneViewModel>
            {
                ValidationMessage = "Please scan or search area."
            });

            _bin.Validations.Add(new IsNotNullRule<BinItemQuantityViewModel>
            {
                ValidationMessage = "Please scan or search location."
            });

            _pickQty.Validations.Add(new IsNotNullRule<int?>
            {
                ValidationMessage = "Please enter pick qty."
            });
        }

        private bool AreFieldsValid()
        {
            _zone.Validate();
            _bin.Validate();
            _pickQty.Validate();

            return _zone.IsValid && _bin.IsValid
                && _pickQty.IsValid;
        }

        private void OnSelectedItem(OrderLinePickLookupViewModel e)
        {
            try
            {
                IsBusy = true;

                OrderLine = e;
                Title = $"Picking: {SaleOrderNo}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnSelectedZone(ZoneItemQuantityViewModel e)
        {
            Zone.Value = new ZoneViewModel
            {
                ZoneId = e.ZoneId,
                Name = e.Name
            };
            _zone.Validate();

            Bin.Value = null;
        }

        private void OnSelectedBin(BinItemQuantityViewModel e)
        {
            Bin.Value = e;
            _bin.Validate();
        }

        private void OnScanResult(BinItemQuantityViewModel e)
        {
            Bin.Value = e;
            _bin.Validate();
        }

        private async Task SearchZone()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PickingZoneLookupViewModel>($"id={OrderLine.ItemId}");
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
                    await _navigationService.PushAsync<PickingBinScanViewModel>($"zid={Zone.Value.ZoneId}&iid={OrderLine.ItemId}");
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

                if (Zone.Value is null)
                {
                    await _dialogService.DisplayAlert("Warning", "Please select area first.", "OK");
                }
                else
                {
                    await _navigationService.PushAsync<PickingBinLookupViewModel>($"zid={Zone.Value.ZoneId}&iid={OrderLine.ItemId}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        void BinUnfocused()
            => _bin.Validate();

        void PickQtyUnfocused()
            => _pickQty.Validate();

        private async Task Save()
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
                var result = await _orderLineService
                    .CreateOrderLineBinDeviceAsync(state.DeviceState, new OrderLineBinCreateModel
                    {
                        UserId = state.AppState.UserId,
                        OrderLineId = OrderLine.OrderLineId,
                        BinId = Bin.Value.BinId,
                        Qty = PickQty.Value
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        var result2 = await _orderLineService
                            .GetOrderLineDeviceAsync(state.DeviceState, OrderLine.OrderLineId);

                        if (result2.Success)
                        {
                            //OrderLine.Received = OrderLine.Received + PickQty.Value.Value;
                            //OrderLine.Remaining = OrderLine.Remaining - PickQty.Value.Value;
                            OrderLine.Received = result2.Response.Received;
                            OrderLine.Remaining = result2.Response.Remaining;

                            _reloadViewEventManager2.RaiseEvent(result2.Response, nameof(ReloadView2));

                            await _dialogService.DisplayAlert("Picking", "Picking Success.", "OK");

                            if (OrderLine.Remaining == 0)
                            {
                                var result3 = await _orderLineService
                                    .GetOrderLinePickLookupDeviceAsync(state.DeviceState, SaleOrderId, null);

                                if (result3.Success)
                                {
                                    _reloadViewEventManager.RaiseEvent(result3.Response, nameof(ReloadView));
                                }

                                await _navigationService.PopAsync();
                            }
                            else
                            {
                                Zone.Value = null;
                                Bin.Value = null;
                                PickQty.Value = null;
                            }
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

        public static event Action<IEnumerable<OrderLinePickLookupGetModel>> ReloadView
        {
            add => _reloadViewEventManager.AddEventHandler(value);
            remove => _reloadViewEventManager.RemoveEventHandler(value);
        }

        public static event Action<OrderLineGetModel> ReloadView2
        {
            add => _reloadViewEventManager2.AddEventHandler(value);
            remove => _reloadViewEventManager2.RemoveEventHandler(value);
        }

        #endregion

        public PickingViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IOrderLineService orderLineService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _orderLineService = orderLineService;
            _unauthorizedService = unauthorizedService;

            PickQueueItemLookupViewModel.SelectedItem += OnSelectedItem;

            PickingZoneLookupViewModel.SelectedZone += OnSelectedZone;
            PickingBinLookupViewModel.SelectedBin += OnSelectedBin;

            PickingBinScanViewModel.ScanResult += OnScanResult;

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