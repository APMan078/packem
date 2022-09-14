using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
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

namespace Packem.Mobile.Modules.Transfers
{
    [QueryProperty("ZoneId", "id")]
    [QueryProperty("ItemId", "iid")]
    public class TransferRequestBinToLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IBinService _binService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<BinItemQuantityLotViewModel> _selectedItemEventManager =
            new WeakEventManager<BinItemQuantityLotViewModel>();

        private int zoneId;
        private int itemId;
        private bool hasRecord;

        #endregion

        #region "Properties"

        public int ZoneId
        {
            get => zoneId;
            set => SetProperty(ref zoneId, value);

        }

        public int ItemId
        {
            get => itemId;
            set => SetProperty(ref itemId, value);
        }

        public ObservableRangeCollection<BinItemQuantityLotViewModel> Bins { get; }

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

        private IEnumerable<BinItemQuantityLotViewModel> DtoToVm(IEnumerable<BinLookupItemQuantityLotDeviceGetModel> dto)
        {
            var vm = new List<BinItemQuantityLotViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new BinItemQuantityLotViewModel
                {
                    BinId = x.BinId,
                    Name = x.Name,
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
                var result = await _binService
                    .GetBinItemQuantityLotLookupDeviceAsync(state.DeviceState, ZoneId, ItemId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            Bins.Clear();
                            Bins.AddRange(DtoToVm(result.Response));
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

                var selectedItem = e.AddedItems[0] as BinItemQuantityLotViewModel;

                if (selectedItem != null)
                {
                    _selectedItemEventManager.RaiseEvent(selectedItem, nameof(SelectedBin));
                    await _navigationService.PopAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<BinItemQuantityLotViewModel> SelectedBin
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        #endregion

        public TransferRequestBinToLookupViewModel(INavigationService navigationService,
            IBinService binService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _binService = binService;
            _unauthorizedService = unauthorizedService;

            Bins = new ObservableRangeCollection<BinItemQuantityLotViewModel>();
            BindingBase.EnableCollectionSynchronization(Bins, null, ObservableCollectionCallback);
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
                var result = await _binService
                    .GetBinItemQuantityLotLookupDeviceAsync(state.DeviceState, ZoneId, ItemId, null);

                Bins.CollectionChanged += (sender, args) =>
                {
                    if (Bins.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        Bins.Clear();
                        Bins.AddRange(DtoToVm(result.Response));
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