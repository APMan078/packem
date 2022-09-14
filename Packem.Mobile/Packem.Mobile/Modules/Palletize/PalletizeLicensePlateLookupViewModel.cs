using Newtonsoft.Json;
using Packem.Domain.Common.Enums;
using Packem.Domain.Entities;
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
    public class PalletizeLicensePlateLookupViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly ILicensePlateService _licensePlateService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<LicensePlateViewModel> _selectedItemEventManager =
            new WeakEventManager<LicensePlateViewModel>();
        private static readonly WeakEventManager<IEnumerable<PalletizeItemViewModel>> _selectedLPItemEventManager =
            new WeakEventManager<IEnumerable<PalletizeItemViewModel>>();

        private bool hasRecord;

        #endregion

        #region "Properties"

        public ObservableRangeCollection<LicensePlateViewModel> LPs { get; }

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

        private IEnumerable<LicensePlateViewModel> DtoToVm(IEnumerable<LicensePlateLookupDeviceGetModel> dto)
        {
            var vm = new List<LicensePlateViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new LicensePlateViewModel
                {
                    LicensePlateId = x.LicensePlateId,
                    LicensePlateNo = x.LicensePlateNo,
                    LicensePlateType = x.LicensePlateType
                });
            }

            return vm;
        }

        private IEnumerable<PalletizeItemViewModel> DtoToVm(LicensePlateKnownAssignmentDeviceGetModel dto)
        {
            var vm = new List<PalletizeItemViewModel>();

            foreach (var x in dto.Products)
            {
                var pi = new PalletizeItemViewModel();
                pi.LicensePlateItemId = x.LicensePlateItemId;
                pi.ItemId = x.ItemId;
                pi.ItemSKU = x.ItemSKU;
                pi.ItemDescription = x.ItemDescription;
                pi.VendorId = x.VendorId;
                pi.VendorName = x.VendorName;
                pi.LotId = x.LotId;
                pi.LotNo = x.LotNo;
                pi.ReferenceNo = x.ReferenceNo;
                pi.Cases = x.Cases;
                pi.EACase = x.EaCase;
                pi.TotalQty = x.TotalQty;
                pi.TotalWgt = x.TotalWeight;

                if (pi.VendorId != null)
                {
                    var v = new ViewModels.Core.VendorViewModel
                    {
                        VendorId = pi.VendorId.Value,
                        Name = pi.VendorName
                    };

                    pi.Vendor = v;

                    pi.Vendors.Clear();
                    pi.Vendors.Add(v);
                }

                if (pi.LotId != null)
                {
                    var lot = new LotViewModel
                    {
                        LotId = pi.LotId.Value,
                        LotNo = pi.LotNo
                    };

                    pi.Lots.Clear();
                    pi.Lots.Add(lot);
                    pi.Lot = lot;
                }

                vm.Add(pi);
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
                var result = await _licensePlateService
                    .GetLicensePlateLookupDeviceAsync(state.DeviceState, searchText);

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        if (App.AppSettings.IsDevelopment)
                        {
                            await Task.Delay(200);
                        }

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            LPs.Clear();
                            LPs.AddRange(DtoToVm(result.Response));
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

                var selectedItem = e.AddedItems[0] as LicensePlateViewModel;

                if (selectedItem != null)
                {
                    _selectedItemEventManager.RaiseEvent(selectedItem, nameof(SelectedItem));

                    if (selectedItem.LicensePlateType == LicensePlateTypeEnum.Known)
                    {
                        var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                        var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                        var result = await _licensePlateService
                            .GetLicensePlateKnownAssignmentDeviceAsync(state.DeviceState, selectedItem.LicensePlateId);

                        await _unauthorizedService
                            .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                            {
                                _selectedLPItemEventManager.RaiseEvent(DtoToVm(result.Response), nameof(SelectedLPItem));
                            });
                    }

                    await _navigationService.PopAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<LicensePlateViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        public static event Action<IEnumerable<PalletizeItemViewModel>> SelectedLPItem
        {
            add => _selectedLPItemEventManager.AddEventHandler(value);
            remove => _selectedLPItemEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PalletizeLicensePlateLookupViewModel(INavigationService navigationService,
            ILicensePlateService licensePlateService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _licensePlateService = licensePlateService;
            _unauthorizedService = unauthorizedService;

            LPs = new ObservableRangeCollection<LicensePlateViewModel>();
            BindingBase.EnableCollectionSynchronization(LPs, null, ObservableCollectionCallback);
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
                var result = await _licensePlateService
                    .GetLicensePlateLookupDeviceAsync(state.DeviceState, null);

                LPs.CollectionChanged += (sender, args) =>
                {
                    if (LPs.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        LPs.Clear();
                        LPs.AddRange(DtoToVm(result.Response));
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