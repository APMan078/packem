using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Transfers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Transfers
{
    public class TransferItemScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ITransferService _transferService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<IEnumerable<TransferItemViewModel>> _scanResultEventManager =
            new WeakEventManager<IEnumerable<TransferItemViewModel>>();

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
                    Remaining = x.Remaining
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
                var result = await _transferService
                    .GetTransferLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode, true);

                if (result.Success)
                {
                    if (result.Response.Count() > 0)
                    {
                        _scanResultEventManager.RaiseEvent(DtoToVm(result.Response), nameof(ScanResult));
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

        public static event Action<IEnumerable<TransferItemViewModel>> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public TransferItemScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            ITransferService transferService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _transferService = transferService;
            _barcodeService = barcodeService;
        }
    }
}