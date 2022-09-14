using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.PurchaseOrders;
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

namespace Packem.Mobile.Modules.PurchaseOrders
{
    public class OrderLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<PurchaseOrderViewModel> _selectedOrderEventManager =
            new WeakEventManager<PurchaseOrderViewModel>();

        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<PurchaseOrderViewModel> PurchaseOrderLookups { get; }

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

        private IEnumerable<PurchaseOrderViewModel> DtoToVm(IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel> dto)
        {
            var vm = new List<PurchaseOrderViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new PurchaseOrderViewModel
                {
                    PurchaseOrderId = x.PurchaseOrderId,
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    VendorNo = x.VendorNo,
                    VendorName = x.VendorName
                });
            }

            return vm;
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
                var result = await _purchaseOrderService
                    .GetPurchaseOrderLookupPOReceiveDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            PurchaseOrderLookups.Clear();
                            PurchaseOrderLookups.AddRange(DtoToVm(result.Response));
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

                var selectedOrder = e.AddedItems[0] as PurchaseOrderViewModel;

                if (selectedOrder != null)
                {
                    _selectedOrderEventManager.RaiseEvent(selectedOrder, nameof(SelectedOrder));
                    await _navigationService.PopAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<PurchaseOrderViewModel> SelectedOrder
        {
            add => _selectedOrderEventManager.AddEventHandler(value);
            remove => _selectedOrderEventManager.RemoveEventHandler(value);
        }

        #endregion

        public OrderLookupViewModel(INavigationService navigationService,
            IPurchaseOrderService purchaseOrderService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _purchaseOrderService = purchaseOrderService;
            _unauthorizedService = unauthorizedService;

            PurchaseOrderLookups = new ObservableRangeCollection<PurchaseOrderViewModel>();
            BindingBase.EnableCollectionSynchronization(PurchaseOrderLookups, null, ObservableCollectionCallback);
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
                var result = await _purchaseOrderService
                    .GetPurchaseOrderLookupPOReceiveDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                PurchaseOrderLookups.CollectionChanged += (sender, args) =>
                {
                    if (PurchaseOrderLookups.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        PurchaseOrderLookups.Clear();
                        PurchaseOrderLookups.AddRange(DtoToVm(result.Response));
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