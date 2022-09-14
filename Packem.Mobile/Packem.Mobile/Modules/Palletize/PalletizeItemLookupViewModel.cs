using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using Packem.Mobile.ViewModels.Palletize;
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

namespace Packem.Mobile.Modules.Palletize
{
    public class PalletizeItemLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IItemService _itemService;
        private readonly IVendorService _vendorService;
        private readonly ILotService _lotService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<ItemVendorViewModel> _selectedInventoryEventManager =
            new WeakEventManager<ItemVendorViewModel>();

        private string searchText;
        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<ItemViewModel> Items { get; }

        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

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

        public ICommand ScanItemCommand
            => new AsyncCommand(ScanItem,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchCommand
            => new AsyncCommand(Search,
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
                    QtyOnHand = x.QtyOnHand
                });
            }

            return vm;
        }

        private IEnumerable<VendorViewModel> DtoToVm(IEnumerable<VendorLookupNameGetModel> dto)
        {
            var vm = new List<VendorViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new VendorViewModel
                {
                    VendorId = x.VendorId,
                    Name = x.Name
                });
            }

            return vm;
        }

        private IEnumerable<LotViewModel> DtoToVm2(IEnumerable<LotLookupGetModel> dto)
        {
            var vm = new List<LotViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new LotViewModel
                {
                    LotId = x.LotId,
                    LotNo = x.LotNo,
                    ExpirationDate = x.ExpirationDate
                });
            }

            return vm;
        }

        private async Task ScanItem()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<PalletizeItemScanViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Search()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _itemService
                    .GetItemLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, SearchText);

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
                    var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                    var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                    var result = await _vendorService
                        .GetVendorLookupByNameDeviceAsync(state.DeviceState, selectedItem.ItemId, null);

                    var result2 = await _lotService
                        .GetLotLookupByItemIdDeviceAsync(state.DeviceState, selectedItem.ItemId, null);

                    await _unauthorizedService
                        .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                        {
                            var itemVendor = new ItemVendorViewModel();
                            itemVendor.Item = selectedItem;

                            itemVendor.Vendors.Clear();
                            itemVendor.Vendors.AddRange(DtoToVm(result.Response));

                            itemVendor.Lots.Clear();
                            itemVendor.Lots.AddRange(DtoToVm2(result2.Response));

                            _selectedInventoryEventManager.RaiseEvent(itemVendor, nameof(SelectedInventory));
                        });

                    await _navigationService.PopAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<ItemVendorViewModel> SelectedInventory
        {
            add => _selectedInventoryEventManager.AddEventHandler(value);
            remove => _selectedInventoryEventManager.RemoveEventHandler(value);
        }

        private void OnScanResult(ItemViewModel e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Items.Clear();
                Items.Add(e);
            });
        }

        #endregion

        public PalletizeItemLookupViewModel(INavigationService navigationService,
            IItemService itemService,
            IVendorService vendorService,
            ILotService lotService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _itemService = itemService;
            _vendorService = vendorService;
            _lotService = lotService;
            _unauthorizedService = unauthorizedService;

            Items = new ObservableRangeCollection<ItemViewModel>();
            BindingBase.EnableCollectionSynchronization(Items, null, ObservableCollectionCallback);

            PalletizeItemScanViewModel.ScanResult += OnScanResult;
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