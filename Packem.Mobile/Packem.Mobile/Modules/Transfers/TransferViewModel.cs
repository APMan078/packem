using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Transfers;
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
    public class TransferViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly ITransferService _transferService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<TransferItemViewModel> _selectedItemEventManager =
            new WeakEventManager<TransferItemViewModel>();

        private string searchText;
        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<TransferItemViewModel> TransferLookups { get; }

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

        public ICommand ManualCommand
            => new AsyncCommand(Manual,
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

        private IEnumerable<TransferItemViewModel> DtoToVm(IEnumerable<TransferLookupDeviceGetModel> dto)
        {
            var vm = new List<TransferItemViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new TransferItemViewModel
                {
                    TransferId = x.TransferId,
                    ItemId = x.ItemId,
                    ItemSKU = x.ItemSKU,
                    ItemDescription = x.ItemDescription,
                    ItemUOM = x.ItemUOM,
                    CurrentZone = x.CurrentZone,
                    CurrentZoneId = x.CurrentZoneId,
                    CurrentBin = x.CurrentBin,
                    CurrentBinId = x.CurrentBinId,
                    CurrentBinQty = x.CurrentBinQty,
                    NewZone = x.NewZone,
                    NewZoneId = x.NewZoneId,
                    NewBin = x.NewBin,
                    NewBinId = x.NewBinId,
                    QtyToTransfer = x.QtyToTransfer,
                    Remaining = x.Remaining,
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

                await _navigationService.PushAsync<TransferItemScanViewModel>();
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
                var result = await _transferService
                    .GetTransferLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, SearchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            TransferLookups.Clear();
                            TransferLookups.AddRange(DtoToVm(result.Response));
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

        private async Task Manual()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<TransferManualViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GridTap(GridSelectionChangedEventArgs e)
        {
            try
            {
                IsBusy = true;

                var selectedItem = e.AddedItems[0] as TransferItemViewModel;

                if (selectedItem != null)
                {
                    await _navigationService.PushAsync<TransferRequestViewModel>();
                    _selectedItemEventManager.RaiseEvent(selectedItem, nameof(SelectedItem));
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<TransferItemViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        private void OnScanResult(IEnumerable<TransferItemViewModel> e)
        {
            try
            {
                IsBusy = true;

                Device.BeginInvokeOnMainThread(() => {
                    TransferLookups.Clear();
                    TransferLookups.AddRange(e);
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnReloadView(IEnumerable<TransferLookupDeviceGetModel> e)
        {
            try
            {
                IsBusy = true;

                if (e != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        TransferLookups.Clear();
                        TransferLookups.AddRange(DtoToVm(e));
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public TransferViewModel(INavigationService navigationService,
            ITransferService transferService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _transferService = transferService;
            _unauthorizedService = unauthorizedService;

            TransferLookups = new ObservableRangeCollection<TransferItemViewModel>();
            BindingBase.EnableCollectionSynchronization(TransferLookups, null, ObservableCollectionCallback);

            TransferItemScanViewModel.ScanResult += OnScanResult;
            TransferRequestViewModel.ReloadView += OnReloadView;
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
                var result = await _transferService
                    .GetTransferLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                TransferLookups.CollectionChanged += (sender, args) =>
                {
                    if (TransferLookups.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        TransferLookups.Clear();
                        TransferLookups.AddRange(DtoToVm(result.Response));
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