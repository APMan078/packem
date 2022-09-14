using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Inventories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Inventories
{
    public class InventoryBarcodeScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IItemService _itemService;
        private readonly IBinService _binService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<InventoryLookupSearchViewModel> _scanResultEventManager =
            new WeakEventManager<InventoryLookupSearchViewModel>();

        #endregion

        #region "Properties"
        #endregion

        #region "Commands"

        public ICommand BarcodeScanResultCommand
            => new AsyncCommand<ZXing.Result>(BarcodeScanResult,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        #endregion

        #region "Functions"

        private IEnumerable<BinZoneViewModel> DtoToVm(IEnumerable<BinZoneDeviceGetModel> dto)
        {
            var vm = new List<BinZoneViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new BinZoneViewModel
                {
                    ZoneId = x.ZoneId,
                    Zone = x.Zone,
                    BinId = x.BinId,
                    Bin = x.Bin,
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
                var result = await _itemService
                    .GetItemLookupSkuDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode);

                if (result.Success)
                {
                    var inventory = new InventoryLookupSearchViewModel();
                    inventory.Item = new ItemViewModel
                    {
                        ItemId = result.Response.ItemId,
                        SKU = result.Response.SKU,
                        Description = result.Response.Description,
                        UOM = result.Response.UOM,
                        QtyOnHand = result.Response.QtyOnHand
                    };

                    var result2 = await _binService
                        .GetBinZoneDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, inventory.Item.ItemId);

                    if (result2.Success)
                    {
                        inventory.BinZones.Clear();
                        inventory.BinZones.AddRange(DtoToVm(result2.Response));
                    }

                    _scanResultEventManager.RaiseEvent(inventory, nameof(ScanResult));
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

        public static event Action<InventoryLookupSearchViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public InventoryBarcodeScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IItemService itemService,
            IBinService binService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _itemService = itemService;
            _binService = binService;
            _barcodeService = barcodeService;
        }
    }
}