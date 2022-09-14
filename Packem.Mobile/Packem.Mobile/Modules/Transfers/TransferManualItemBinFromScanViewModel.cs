using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using System;
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
    [QueryProperty("ZoneId", "zid")]
    [QueryProperty("ItemId", "iid")]
    public class TransferManualItemBinFromScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IBinService _binService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<BinItemQuantityViewModel> _scanResultEventManager =
            new WeakEventManager<BinItemQuantityViewModel>();

        private int zoneId;
        private int itemId;

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

        #endregion

        #region "Commands"

        public ICommand BarcodeScanResultCommand
           => new AsyncCommand<ZXing.Result>(BarcodeScanResult,
               canExecute: x => IsNotBusy,
               onException: x => Console.WriteLine(x),
               allowsMultipleExecutions: false);

        #endregion

        #region "Functions"

        private IEnumerable<BinItemQuantityViewModel> DtoToVm(IEnumerable<BinLookupItemQuantityGetModel> dto)
        {
            var vm = new List<BinItemQuantityViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new BinItemQuantityViewModel
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

        private async Task BarcodeScanResult(ZXing.Result r)
        {
            try
            {
                IsBusy = true;

                Barcode = r.Text;
                _barcodeService.PlaySound();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _binService
                    .GetBinLookupItemQuantityDevice(state.DeviceState, state.Facility.CustomerFacilityId, ItemId, ZoneId, Barcode, true);

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
                            await _dialogService.DisplayAlert("Warning", "Location not found.", "OK");
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

        public static event Action<BinItemQuantityViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public TransferManualItemBinFromScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IBinService binService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _binService = binService;
            _barcodeService = barcodeService;
        }
    }
}