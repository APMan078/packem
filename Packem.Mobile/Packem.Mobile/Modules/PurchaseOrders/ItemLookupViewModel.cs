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
    [QueryProperty("PurchaseOrderId", "id")]
    public class ItemLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IReceiveService _receiveService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<ReceiveViewModel> _selectedItemEventManager =
            new WeakEventManager<ReceiveViewModel>();

        private int purchaseOrderId;
        private bool hasRecord;

        #endregion

        #region "Properties"

        public int PurchaseOrderId
        {
            get => purchaseOrderId;
            set => SetProperty(ref purchaseOrderId, value);
        }

        public ObservableRangeCollection<ReceiveViewModel> Items { get; }

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

        private IEnumerable<ReceiveViewModel> DtoToVm(IEnumerable<ReceiveLookupPOReceiveDeviceGetModel> dto)
        {
            var vm = new List<ReceiveViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new ReceiveViewModel
                {
                    ReceiveId = x.ReceiveId,
                    ItemId = x.ItemId,
                    SKU = x.SKU,
                    Description = x.Description,
                    UOM = x.UOM,
                    Remaining = x.Remaining
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
                var result = await _receiveService
                    .GetReceiveLookupPOReceiveDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, PurchaseOrderId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            Items.Clear();
                            Items.AddRange(DtoToVm(result.Response));
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

                var selectedItem = e.AddedItems[0] as ReceiveViewModel;

                if (selectedItem != null)
                {
                    _selectedItemEventManager.RaiseEvent(selectedItem, nameof(SelectedItem));
                    await _navigationService.PopAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<ReceiveViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        #endregion

        public ItemLookupViewModel(INavigationService navigationService,
            IReceiveService receiveService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _receiveService = receiveService;
            _unauthorizedService = unauthorizedService;

            Items = new ObservableRangeCollection<ReceiveViewModel>();
            BindingBase.EnableCollectionSynchronization(Items, null, ObservableCollectionCallback);
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
                var result = await _receiveService
                    .GetReceiveLookupPOReceiveDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, PurchaseOrderId, null);

                Items.CollectionChanged += (sender, args) =>
                {
                    if (Items.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        Items.Clear();
                        Items.AddRange(DtoToVm(result.Response));
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