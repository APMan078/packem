using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.PutAways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.PutAways
{
    public class PutAwayLicensePlateScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPutAwayService _putAwayService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<PutAwayLicensePlateViewModel> _scanResultEventManager =
            new WeakEventManager<PutAwayLicensePlateViewModel>();

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

        private IEnumerable<PutAwayLicensePlateViewModel> DtoToVm(IEnumerable<PutAwayLookupLicensePlateDeviceGetModel> dto)
        {
            var vm = new List<PutAwayLicensePlateViewModel>();

            foreach (var x in dto)
            {
                var items = new List<PutAwayLicensePlateItemViewModel>();
                foreach (var z in x.Items)
                {
                    items.Add(new PutAwayLicensePlateItemViewModel
                    {
                        ItemId = z.ItemId,
                        SKU = z.ItemSKU,
                        Description = z.ItemDescription,
                        TotalQty = z.TotalQty
                    });
                }

                var p = new PutAwayLicensePlateViewModel();
                p.PutAwayId = x.PutAwayId;
                p.LicensePlateId = x.LicensePlateId;
                p.LicensePlateNo = x.LicensePlateNo;
                p.LicensePlateType = x.LicensePlateType;
                p.Items.Clear();
                p.Items.AddRange(items);

                vm.Add(p);
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
                var result = await _putAwayService
                    .GetPutAwayLookupLicensePlateDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode, true);

                if (result.Success)
                {
                    if (result.Response.Count() > 0)
                    {
                        _scanResultEventManager.RaiseEvent(DtoToVm(result.Response).FirstOrDefault(), nameof(ScanResult));
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

        public static event Action<PutAwayLicensePlateViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PutAwayLicensePlateScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IPutAwayService putAwayService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _putAwayService = putAwayService;
            _barcodeService = barcodeService;
        }
    }
}