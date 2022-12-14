using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Inventories;
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

namespace Packem.Mobile.Modules.Inventories
{
    public class InventoryLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IItemService _itemService;
        private readonly IBinService _binService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<InventoryLookupSearchViewModel> _selectedInventoryEventManager =
            new WeakEventManager<InventoryLookupSearchViewModel>();

        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<ItemViewModel> Items { get; }

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

        private IEnumerable<ItemViewModel> DtoToVm(IEnumerable<ItemLookupDeviceGetModel> dto)
        {
            var vm = new List<ItemViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new ItemViewModel
                {
                    ItemId = x.ItemId,
                    SKU = x.SKU,
                    Description = x.Description,
                    UOM = x.UOM,
                    QtyOnHand = x.QtyOnHand,
                    OnOrder = x.OnOrder
                });
            }

            return vm;
        }

        private IEnumerable<BinZoneViewModel> DtoToVm(IEnumerable<BinZoneDeviceGetModel> dto)
        {
            var vm = new List<BinZoneViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new BinZoneViewModel
                {
                    ZoneId = x.ZoneId,
                    Zone = x.Zone,
                    BinId = x.BinId,
                    Bin = x.Bin,
                    Qty = x.Qty,
                    LotNo = x.LotNo,
                    ExpirationDate = x.ExpirationDate
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
                var result = await _itemService
                    .GetItemLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() =>
                        {
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

                var selectedItem = e.AddedItems[0] as ItemViewModel;

                if (selectedItem != null)
                {
                    var inventory = new InventoryLookupSearchViewModel();
                    inventory.Item = selectedItem;

                    var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                    var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                    var result = await _binService
                        .GetBinZoneDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, inventory.Item.ItemId);

                    if (result.Success)
                    {
                        inventory.BinZones.Clear();
                        inventory.BinZones.AddRange(DtoToVm(result.Response));
                    }

                    _selectedInventoryEventManager.RaiseEvent(inventory, nameof(SelectedInventory));
                    await _navigationService.PopAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<InventoryLookupSearchViewModel> SelectedInventory
        {
            add => _selectedInventoryEventManager.AddEventHandler(value);
            remove => _selectedInventoryEventManager.RemoveEventHandler(value);
        }

        #endregion

        public InventoryLookupViewModel(INavigationService navigationService,
            IItemService itemService,
            IBinService binService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _itemService = itemService;
            _binService = binService;
            _unauthorizedService = unauthorizedService;

            Items = new ObservableRangeCollection<ItemViewModel>();
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
                var result = await _itemService
                    .GetItemLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

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
