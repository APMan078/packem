using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Core;
using Packem.Mobile.ViewModels.Picking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Picking
{
    [QueryProperty("SaleOrderId", "id")]
    public class PickQueueItemScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IOrderLineService _orderLineService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<IEnumerable<OrderLinePickLookupViewModel>> _scanResultEventManager =
            new WeakEventManager<IEnumerable<OrderLinePickLookupViewModel>>();

        private int saleOrderId;

        #endregion

        #region "Properties"

        public int SaleOrderId
        {
            get => saleOrderId;
            set => SetProperty(ref saleOrderId, value);

        }

        #endregion

        #region "Commands"

        public ICommand BarcodeScanResultCommand
            => new AsyncCommand<ZXing.Result>(BarcodeScanResult,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        #endregion

        #region "Functions"

        private IEnumerable<OrderLinePickLookupViewModel> DtoToVm(IEnumerable<OrderLinePickLookupGetModel> dto)
        {
            var vm = new List<OrderLinePickLookupViewModel>();

            foreach (var x in dto)
            {
                var ol = new OrderLinePickLookupViewModel();
                ol.OrderLineId = x.OrderLineId;
                ol.ItemId = x.ItemId;
                ol.ItemSKU = x.ItemSKU;
                ol.ItemDescription = x.ItemDescription;
                ol.ItemUOM = x.ItemUOM;
                ol.Qty = x.Qty;
                ol.Received = x.Received;
                ol.Remaining = x.Remaining;

                var bins = new List<BinViewModel>();
                foreach (var z in x.Bins)
                {
                    bins.Add(new BinViewModel
                    {
                        BinId = z.BinId,
                        Name = z.Name
                    });
                }

                ol.Bins.Clear();
                ol.Bins.AddRange(bins);
                vm.Add(ol);
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
                var result = await _orderLineService
                    .GetOrderLinePickLookupDeviceAsync(state.DeviceState, SaleOrderId, Barcode, true);

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

        public static event Action<IEnumerable<OrderLinePickLookupViewModel>> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PickQueueItemScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IOrderLineService orderLineService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _orderLineService = orderLineService;
            _barcodeService = barcodeService;
        }
    }
}