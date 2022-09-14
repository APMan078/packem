using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using Packem.Mobile.ViewModels.Picking;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Picking
{
    [QueryProperty("SaleOrderId", "id")]
    public class PickQueueItemLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IOrderLineService _orderLineService;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<OrderLinePickLookupViewModel> _selectedItemEventManager =
            new WeakEventManager<OrderLinePickLookupViewModel>();
        private static readonly WeakEventManager<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>> _reloadViewEventManager =
            new WeakEventManager<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>>();

        private int saleOrderId;
        private SaleOrderPickQueueLookupViewModel saleOrder;
        private string title;
        private bool hasRecord;
        private bool startVisible;
        private bool pauseVisible;
        private bool completeVisible;

        #endregion

        #region "Properties"

        public int SaleOrderId
        {
            get => saleOrderId;
            set => SetProperty(ref saleOrderId, value);

        }

        public SaleOrderPickQueueLookupViewModel SaleOrder
        {
            get => saleOrder;
            set => SetProperty(ref saleOrder, value);

        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);

        }

        public ObservableRangeCollection<OrderLinePickLookupViewModel> OrderLines { get; }

        public bool HasRecord
        {
            get => hasRecord;
            set => SetProperty(ref hasRecord, value);
        }

        public bool StartVisible
        {
            get => startVisible;
            set => SetProperty(ref startVisible, value);
        }

        public bool PauseVisible
        {
            get => pauseVisible;
            set => SetProperty(ref pauseVisible, value);
        }

        public bool CompleteVisible
        {
            get => completeVisible;
            set => SetProperty(ref completeVisible, value);
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand ScanCommand
            => new AsyncCommand(Scan,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchCommand
            => new AsyncCommand<string>(Search,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand GridTapCommand
            => new AsyncCommand<GridSelectionChangedEventArgs>(GridTap,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand StartCommand
            => new AsyncCommand(Start,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand PauseCommand
            => new AsyncCommand(Pause,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

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

        private void UpdatePickingStatus()
        {
            if (SaleOrder.PickingStatus == Domain.Common.Enums.PickingStatusEnum.Pending
                    || SaleOrder.PickingStatus == Domain.Common.Enums.PickingStatusEnum.Pause)
            {
                Title = "Pick";
                StartVisible = true;
                PauseVisible = false;
                CompleteVisible = false;
            }
            else
            {
                Title = $"Picking: {SaleOrder.SaleOrderNo}";
                StartVisible = false;
                PauseVisible = true;
                CompleteVisible = false;
            }

            var completed = true;
            foreach (var x in OrderLines)
            {
                if (!x.Completed)
                {
                    completed = false;
                }
            }

            if (completed)
            {
                CompleteVisible = true;
            }
        }

        private void OnSelectedItem(SaleOrderPickQueueLookupViewModel e)
        {
            try
            {
                IsBusy = true;

                SaleOrder = e;
                UpdatePickingStatus();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private IEnumerable<OrderLinePickLookupViewModel> DtoToVm(IEnumerable<OrderLinePickLookupGetModel> dto)
        {
            var vm = new List<OrderLinePickLookupViewModel>();

            foreach (var x in dto)
            {
                var ol = new OrderLinePickLookupViewModel();
                ol.OrderLineId = x.OrderLineId;
                ol.ItemId = x.ItemId;
                ol.ItemSKU = x.ItemSKU;
                ol.ItemDescription = x.ItemDescription;
                ol.ItemUOM = x.ItemUOM;
                ol.Qty = x.Qty;
                ol.Received = x.Received;
                ol.Remaining = x.Remaining;

                if (ol.Qty == ol.Received)
                {
                    ol.Completed = true;
                }

                var bins = new List<BinViewModel>();
                foreach (var z in x.Bins)
                {
                    bins.Add(new BinViewModel
                    {
                        BinId = z.BinId,
                        Name = z.Name
                    });
                }

                ol.Bins.Clear();
                ol.Bins.AddRange(bins);
                vm.Add(ol);
            }

            return vm;
        }

        private async Task Scan()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PickQueueItemScanViewModel>($"id={SaleOrderId}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Search(string searchText)
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _orderLineService
                    .GetOrderLinePickLookupDeviceAsync(state.DeviceState, SaleOrderId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            OrderLines.Clear();
                            OrderLines.AddRange(DtoToVm(result.Response));
                        });
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private async Task GridTap(GridSelectionChangedEventArgs e)
        {
            try
            {
                IsBusy = true;

                var selectedItem = e.AddedItems[0] as OrderLinePickLookupViewModel;

                if (selectedItem != null)
                {
                    if (SaleOrder.PickingStatus == Domain.Common.Enums.PickingStatusEnum.Picking)
                    {
                        await _navigationService.PushAsync<PickingViewModel>($"id={SaleOrderId}&sNo={SaleOrder.SaleOrderNo}");
                        _selectedItemEventManager.RaiseEvent(selectedItem, nameof(SelectedItem));
                    }
                    else
                    {
                        await _dialogService.DisplayAlert("Warning", "Click START to do picking.", "OK");
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Start()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _saleOrderService
                    .UpdateSaleOrderPickingStatusDeviceAsync(state.DeviceState, new SaleOrderPickingStatusUpdateModel
                    {
                        SaleOrderId = SaleOrderId,
                        PickingStatus = Domain.Common.Enums.PickingStatusEnum.Picking
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        SaleOrder.PickingStatus = Domain.Common.Enums.PickingStatusEnum.Picking;
                        UpdatePickingStatus();
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private async Task Pause()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _saleOrderService
                    .UpdateSaleOrderPickingStatusDeviceAsync(state.DeviceState, new SaleOrderPickingStatusUpdateModel
                    {
                        SaleOrderId = SaleOrderId,
                        PickingStatus = Domain.Common.Enums.PickingStatusEnum.Pause
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        SaleOrder.PickingStatus = Domain.Common.Enums.PickingStatusEnum.Pause;
                        UpdatePickingStatus();
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private async Task Complete()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _saleOrderService
                    .UpdateSaleOrderPickingStatusDeviceAsync(state.DeviceState, new SaleOrderPickingStatusUpdateModel
                    {
                        SaleOrderId = SaleOrderId,
                        PickingStatus = Domain.Common.Enums.PickingStatusEnum.Complete
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        SaleOrder.PickingStatus = Domain.Common.Enums.PickingStatusEnum.Complete;
                        await _dialogService.DisplayAlert("Picking", "Picking Completed.", "OK");

                        var result2 = await _saleOrderService
                            .GetSaleOrderPickQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                        if (result2.Success)
                        {
                            _reloadViewEventManager.RaiseEvent(result2.Response, nameof(ReloadView));
                        }

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

        public static event Action<OrderLinePickLookupViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        private void OnScanResult(IEnumerable<OrderLinePickLookupViewModel> e)
        {
            try
            {
                IsBusy = true;

                Device.BeginInvokeOnMainThread(() => {
                    OrderLines.Clear();
                    OrderLines.AddRange(e);
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnReloadView(IEnumerable<OrderLinePickLookupGetModel> e)
        {
            try
            {
                IsBusy = true;

                if (e != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        OrderLines.Clear();
                        OrderLines.AddRange(DtoToVm(e));
                        UpdatePickingStatus();
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnReloadView2(OrderLineGetModel e)
        {
            try
            {
                IsBusy = true;

                if (e != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        var ol = OrderLines.SingleOrDefault(x => x.OrderLineId == e.OrderLineId);
                        ol.Received = e.Received;
                        ol.Remaining = e.Remaining;

                        var index = OrderLines.IndexOf(ol);
                        if (index != -1)
                        {
                            OrderLines.RemoveAt(index);
                            OrderLines.Insert(index, ol);
                        }
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<IEnumerable<SaleOrderPickQueueLookupDeviceGetModel>> ReloadView
        {
            add => _reloadViewEventManager.AddEventHandler(value);
            remove => _reloadViewEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PickQueueItemLookupViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IOrderLineService orderLineService,
            ISaleOrderService saleOrderService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _orderLineService = orderLineService;
            _saleOrderService = saleOrderService;
            _unauthorizedService = unauthorizedService;

            OrderLines = new ObservableRangeCollection<OrderLinePickLookupViewModel>();
            BindingBase.EnableCollectionSynchronization(OrderLines, null, ObservableCollectionCallback);

            PickQueueViewModel.SelectedItem += OnSelectedItem;
            PickQueueItemScanViewModel.ScanResult += OnScanResult;
            PickingViewModel.ReloadView += OnReloadView;
            PickingViewModel.ReloadView2 += OnReloadView2;
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

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _orderLineService
                    .GetOrderLinePickLookupDeviceAsync(state.DeviceState, SaleOrderId, null);

                OrderLines.CollectionChanged += (sender, args) =>
                {
                    if (OrderLines.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        OrderLines.Clear();
                        OrderLines.AddRange(DtoToVm(result.Response));
                    });
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