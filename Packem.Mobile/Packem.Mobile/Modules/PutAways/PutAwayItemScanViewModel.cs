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
    public class PutAwayItemScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPutAwayService _putAwayService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<PutAwayItemViewModel> _scanResultEventManager =
            new WeakEventManager<PutAwayItemViewModel>();

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
                    .GetPutAwayLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode, true);

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
                            await _dialogService.DisplayAlert("Warning", "Item not found.", "OK");
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

        public static event Action<PutAwayItemViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PutAwayItemScanViewModel(INavigationService navigationService,
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