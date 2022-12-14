using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.PutAways;
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

namespace Packem.Mobile.Modules.PutAways
{
    public class PutAwayItemLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IPutAwayService _putAwayService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<PutAwayItemViewModel> _selectedItemEventManager =
            new WeakEventManager<PutAwayItemViewModel>();

        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<PutAwayItemViewModel> PutAwayLookups { get; }

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

        private IEnumerable<PutAwayItemViewModel> DtoToVm(IEnumerable<PutAwayLookupDeviceGetModel> dto)
        {
            var vm = new List<PutAwayItemViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new PutAwayItemViewModel
                {
                    PutAwayId = x.PutAwayId,
                    ItemId = x.ItemId,
                    SKU = x.SKU,
                    Description = x.Description,
                    Remaining = x.Remaining,
                    PutAwayType = x.PutAwayType,
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
                var result = await _putAwayService
                    .GetPutAwayLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() => {
                            PutAwayLookups.Clear();
                            PutAwayLookups.AddRange(DtoToVm(result.Response));
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

                var selectedItem = e.AddedItems[0] as PutAwayItemViewModel;

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

        public static event Action<PutAwayItemViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PutAwayItemLookupViewModel(INavigationService navigationService,
            IPutAwayService putAwayService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _putAwayService = putAwayService;
            _unauthorizedService = unauthorizedService;

            PutAwayLookups = new ObservableRangeCollection<PutAwayItemViewModel>();
            BindingBase.EnableCollectionSynchronization(PutAwayLookups, null, ObservableCollectionCallback);
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
                var result = await _putAwayService
                    .GetPutAwayLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                PutAwayLookups.CollectionChanged += (sender, args) =>
                {
                    if (PutAwayLookups.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        PutAwayLookups.Clear();
                        PutAwayLookups.AddRange(DtoToVm(result.Response));
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
