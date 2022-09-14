using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.PurchaseOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.PurchaseOrders
{
    public class OrderScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<PurchaseOrderViewModel> _scanResultEventManager =
            new WeakEventManager<PurchaseOrderViewModel>();

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

        private IEnumerable<PurchaseOrderViewModel> DtoToVm(IEnumerable<PurchaseOrderLookupPOReceiveDeviceGetModel> dto)
        {
            var vm = new List<PurchaseOrderViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new PurchaseOrderViewModel
                {
                    PurchaseOrderId = x.PurchaseOrderId,
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    VendorNo = x.VendorNo,
                    VendorName = x.VendorName
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
                var result = await _purchaseOrderService
                    .GetPurchaseOrderLookupPOReceiveDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode, true);

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
                            await _dialogService.DisplayAlert("Warning", "PO not found.", "OK");
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

        public static event Action<PurchaseOrderViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public OrderScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IPurchaseOrderService purchaseOrderService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _purchaseOrderService = purchaseOrderService;
            _barcodeService = barcodeService;
        }
    }
}