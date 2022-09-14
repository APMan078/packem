using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Receipts
{
    public class ReceiptItemScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IItemService _itemService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<ItemViewModel> _scanResultEventManager =
            new WeakEventManager<ItemViewModel>();

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

        private ItemViewModel DtoToVm(ItemLookupDeviceGetModel dto)
        {
            return new ItemViewModel
            {
                ItemId = dto.ItemId,
                SKU = dto.SKU,
                Description = dto.Description,
                UOM = dto.UOM,
                QtyOnHand = dto.QtyOnHand
            };
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
                var result = await _itemService
                    .GetItemLookupSkuDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode);

                if (result.Success)
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
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<ItemViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public ReceiptItemScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IItemService itemService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _itemService = itemService;
            _barcodeService = barcodeService;
        }
    }
}