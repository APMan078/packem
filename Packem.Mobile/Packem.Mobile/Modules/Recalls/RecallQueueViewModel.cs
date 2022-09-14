using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Recalls;
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

namespace Packem.Mobile.Modules.Recalls
{
    public class RecallQueueViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IRecallService _recallService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<RecallQueueLookupViewModel> _selectedItemEventManager =
            new WeakEventManager<RecallQueueLookupViewModel>();

        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<RecallQueueLookupViewModel> Recalls { get; }

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

        private IEnumerable<RecallQueueLookupViewModel> DtoToVm(IEnumerable<RecallQueueLookupGetModel> dto)
        {
            var vm = new List<RecallQueueLookupViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new RecallQueueLookupViewModel
                {
                    RecallId = x.RecallId,
                    RecallDate = x.RecallDate,
                    Status = x.Status,
                    ItemSKU = x.ItemSKU,
                    ItemDescription = x.ItemDescription,
                    ItemUOM = x.ItemUOM,
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

                await _navigationService.PushAsync<RecallQueueScanViewModel>();
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
                var result = await _recallService
                    .GetRecallQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            Recalls.Clear();
                            Recalls.AddRange(DtoToVm(result.Response));
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

                var selectedItem = e.AddedItems[0] as RecallQueueLookupViewModel;

                if (selectedItem != null)
                {
                    await _navigationService.PushAsync<RecallItemViewModel>($"id={selectedItem.RecallId}");
                    _selectedItemEventManager.RaiseEvent(selectedItem, nameof(SelectedItem));
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<RecallQueueLookupViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        private void OnScanResult(IEnumerable<RecallQueueLookupViewModel> e)
        {
            try
            {
                IsBusy = true;

                Device.BeginInvokeOnMainThread(() => {
                    Recalls.Clear();
                    Recalls.AddRange(e);
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnReloadView(IEnumerable<RecallQueueLookupGetModel> e)
        {
            try
            {
                IsBusy = true;

                if (e != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        Recalls.Clear();
                        Recalls.AddRange(DtoToVm(e));
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public RecallQueueViewModel(INavigationService navigationService,
            IRecallService recallService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _recallService = recallService;
            _unauthorizedService = unauthorizedService;

            Recalls = new ObservableRangeCollection<RecallQueueLookupViewModel>();
            BindingBase.EnableCollectionSynchronization(Recalls, null, ObservableCollectionCallback);

            RecallQueueScanViewModel.ScanResult += OnScanResult;
            RecallItemViewModel.ReloadView += OnReloadView;
            RecallPickingViewModel.ReloadView2 += OnReloadView;
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
                var result = await _recallService
                    .GetRecallQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                Recalls.CollectionChanged += (sender, args) =>
                {
                    if (Recalls.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        Recalls.Clear();
                        Recalls.AddRange(DtoToVm(result.Response));
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