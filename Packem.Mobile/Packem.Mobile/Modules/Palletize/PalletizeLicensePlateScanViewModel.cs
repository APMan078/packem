using Newtonsoft.Json;
using Packem.Domain.Common.Enums;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Palletize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Palletize
{
    public class PalletizeLicensePlateScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IBarcodeService _barcodeService;
        private readonly ILicensePlateService _licensePlateService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<LicensePlateViewModel> _scanResultEventManager =
            new WeakEventManager<LicensePlateViewModel>();
        private static readonly WeakEventManager<IEnumerable<PalletizeItemViewModel>> _selectedLPItemEventManager =
            new WeakEventManager<IEnumerable<PalletizeItemViewModel>>();

        #endregion

        #region "Properties"
        #endregion

        #region "Commands"

        public ICommand BarcodeScanResultCommand
            => new AsyncCommand<ZXing.Result>(BarcodeScanResult,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        #endregion

        #region "Functions"

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
                vm.Add(new PalletizeItemViewModel
                {
                    LicensePlateItemId = x.LicensePlateItemId,
                    ItemId = x.ItemId,
                    ItemSKU = x.ItemSKU,
                    ItemDescription = x.ItemDescription,
                    Cases = x.Cases,
                    EACase = x.EaCase,
                    TotalQty = x.TotalQty,
                    TotalWgt = x.TotalWeight
                });
            }

            return vm;
        }

        private async Task BarcodeScanResult(ZXing.Result r)
        {
            try
            {
                IsBusy = true;

                Barcode = r.Text;
                _barcodeService.PlaySound();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _licensePlateService
                    .GetLicensePlateLookupDeviceAsync(state.DeviceState, Barcode, true);

                if (result.Success)
                {
                    if (result.Response.Count() > 0)
                    {
                        var lp = DtoToVm(result.Response).FirstOrDefault();

                        _scanResultEventManager.RaiseEvent(lp, nameof(ScanResult));

                        if (lp.LicensePlateType == LicensePlateTypeEnum.Known)
                        {
                            var result2 = await _licensePlateService
                                .GetLicensePlateKnownAssignmentDeviceAsync(state.DeviceState, lp.LicensePlateId);

                            await _unauthorizedService
                                .UnauthorizedWorkflowIfNotAuthenticated(result2, () =>
                                {
                                    _selectedLPItemEventManager.RaiseEvent(DtoToVm(result2.Response), nameof(SelectedLPItem));
                                });
                        }

                        await _navigationService.PopAsync();
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await _dialogService.DisplayAlert("Warning", "License plate not found.", "OK");
                        });

                        await _navigationService.PopAsync();
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<LicensePlateViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        public static event Action<IEnumerable<PalletizeItemViewModel>> SelectedLPItem
        {
            add => _selectedLPItemEventManager.AddEventHandler(value);
            remove => _selectedLPItemEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PalletizeLicensePlateScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IBarcodeService barcodeService,
            ILicensePlateService licensePlateService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _barcodeService = barcodeService;
            _licensePlateService = licensePlateService;
            _unauthorizedService = unauthorizedService;
        }
    }
}