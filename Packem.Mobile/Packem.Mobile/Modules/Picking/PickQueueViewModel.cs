using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
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
    public class PickQueueViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<SaleOrderPickQueueLookupViewModel> _selectedItemEventManager =
            new WeakEventManager<SaleOrderPickQueueLookupViewModel>();

        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<SaleOrderPickQueueLookupViewModel> SaleOrders { get; }

        public bool HasRecord
        {
            get => hasRecord;
            set => SetProperty(ref hasRecord, value);
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

        private IEnumerable<SaleOrderPickQueueLookupViewModel> DtoToVm(IEnumerable<SaleOrderPickQueueLookupDeviceGetModel> dto)
        {
            var vm = new List<SaleOrderPickQueueLookupViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new SaleOrderPickQueueLookupViewModel
                {
                    SaleOrderId = x.SaleOrderId,
                    SaleOrderNo = x.SaleOrderNo,
                    PickingStatus = x.PickingStatus,
                    Items = x.Items,
                    Bins = x.Bins
                });
            }

            return vm;
        }

        private async Task Scan()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PickQueueScanViewModel>();
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
                var result = await _saleOrderService
                    .GetSaleOrderPickQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            SaleOrders.Clear();
                            SaleOrders.AddRange(DtoToVm(result.Response));
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

                var selectedItem = e.AddedItems[0] as SaleOrderPickQueueLookupViewModel;

                if (selectedItem != null)
                {
                    await _navigationService.PushAsync<PickQueueItemLookupViewModel>($"id={selectedItem.SaleOrderId}");
                    _selectedItemEventManager.RaiseEvent(selectedItem, nameof(SelectedItem));
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<SaleOrderPickQueueLookupViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        private void OnScanResult(IEnumerable<SaleOrderPickQueueLookupViewModel> e)
        {
            try
            {
                IsBusy = true;

                Device.BeginInvokeOnMainThread(() => {
                    SaleOrders.Clear();
                    SaleOrders.AddRange(e);
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnReloadView(IEnumerable<SaleOrderPickQueueLookupDeviceGetModel> e)
        {
            try
            {
                IsBusy = true;

                if (e != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        SaleOrders.Clear();
                        SaleOrders.AddRange(DtoToVm(e));
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public PickQueueViewModel(INavigationService navigationService,
            ISaleOrderService saleOrderService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _saleOrderService = saleOrderService;
            _unauthorizedService = unauthorizedService;

            SaleOrders = new ObservableRangeCollection<SaleOrderPickQueueLookupViewModel>();
            BindingBase.EnableCollectionSynchronization(SaleOrders, null, ObservableCollectionCallback);

            PickQueueScanViewModel.ScanResult += OnScanResult;
            PickQueueItemLookupViewModel.ReloadView += OnReloadView;
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
                var result = await _saleOrderService
                    .GetSaleOrderPickQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                SaleOrders.CollectionChanged += (sender, args) =>
                {
                    if (SaleOrders.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        SaleOrders.Clear();
                        SaleOrders.AddRange(DtoToVm(result.Response));
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