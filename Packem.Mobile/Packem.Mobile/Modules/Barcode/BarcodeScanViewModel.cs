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
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Barcode
{
    public class BarcodeScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly IBarcodeService _barcodeService;
        private readonly IDialogService _dialogService;
        private readonly IItemService _itemService;
        private readonly IBinService _binService;
        private static readonly WeakEventManager<InventoryLookupSearchViewModel> _scanResultEventManager =
            new WeakEventManager<InventoryLookupSearchViewModel>();

        private bool _popupOpen;

        #endregion

        #region "Properties"

        public bool PopupOpen
        {
            get => _popupOpen;
            set { SetProperty(ref _popupOpen, value); }
        }

        #endregion

        #region "Commands"

        public ICommand ScanResultCommand
            => new Command<ZXing.Result>(BarcodeScanResult,
                canExecute: (r) => IsNotBusy);

        public ICommand PopupAcceptCommand
            => new Command(ScanResultCancel,
                canExecute: () => IsNotBusy);

        public ICommand InventoryCommand
            => new AsyncCommand(Inventory,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

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
                    Qty = x.Qty
                });
            }

            return vm;
        }

        private void BarcodeScanResult(ZXing.Result r)
        {
            try
            {
                IsBusy = true;

                Barcode = r.Text;
                _barcodeService.PlaySound();

                PopupOpen = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ScanResultCancel()
        {
            try
            {
                IsBusy = true;

                Barcode = null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Inventory()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                await Task.Run(async () =>
                {
                    var inventory = new InventoryLookupSearchViewModel();
                    var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                    var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                    var result = await _itemService
                        .GetItemLookupSkuDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode);
                    var success = result.Success;

                    if (result.Success)
                    {
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
                    }

                    return new { inventory, success };
                })
                    .ContinueWith(t =>
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            PopupOpen = false;

                            await Shell.Current.GoToAsync("//MainMenuViewModel/InventoryViewModel");
                            if (t.Result.success)
                            {
                                _scanResultEventManager.RaiseEvent(t.Result.inventory, nameof(InventoryScanResult));
                            }
                            else
                            {
                                await _dialogService.DisplayAlert("Warning", "Item not found.", "OK");
                            }
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

        public static event Action<InventoryLookupSearchViewModel> InventoryScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public BarcodeScanViewModel(IBarcodeService barcodeService,
            IDialogService dialogService,
            IItemService itemService,
            IBinService binService)
        {
            _barcodeService = barcodeService;
            _dialogService = dialogService;
            _itemService = itemService;
            _binService = binService;

            ActivityIndicatorViewModel
                = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
        }
    }
}