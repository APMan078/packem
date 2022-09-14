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
    [QueryProperty("PurchaseOrderId", "id")]
    public class ItemScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IReceiveService _receiveService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<ReceiveViewModel> _scanResultEventManager
            = new WeakEventManager<ReceiveViewModel>();

        private int purchaseOrderId;

        #endregion

        #region "Properties"

        public int PurchaseOrderId
        {
            get => purchaseOrderId;
            set => SetProperty(ref purchaseOrderId, value);
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

        private IEnumerable<ReceiveViewModel> DtoToVm(IEnumerable<ReceiveLookupPOReceiveDeviceGetModel> dto)
        {
            var vm = new List<ReceiveViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new ReceiveViewModel
                {
                    ReceiveId = x.ReceiveId,
                    ItemId = x.ItemId,
                    SKU = x.SKU,
                    Description = x.Description,
                    UOM = x.UOM,
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
                var result = await _receiveService
                    .GetReceiveLookupPOReceiveDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, PurchaseOrderId, Barcode, true);

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

        public static event Action<ReceiveViewModel> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public ItemScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IReceiveService receiveService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _receiveService = receiveService;
            _barcodeService = barcodeService;
        }
    }
}