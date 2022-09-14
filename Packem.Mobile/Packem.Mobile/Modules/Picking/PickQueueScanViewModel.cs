using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Picking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Picking
{
    public class PickQueueScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<IEnumerable<SaleOrderPickQueueLookupViewModel>> _scanResultEventManager =
            new WeakEventManager<IEnumerable<SaleOrderPickQueueLookupViewModel>>();

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

        private IEnumerable<SaleOrderPickQueueLookupViewModel> DtoToVm(IEnumerable<SaleOrderPickQueueLookupDeviceGetModel> dto)
        {
            var vm = new List<SaleOrderPickQueueLookupViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new SaleOrderPickQueueLookupViewModel
                {
                    SaleOrderId = x.SaleOrderId,
                    SaleOrderNo = x.SaleOrderNo,
                    PickingStatus = x.PickingStatus,
                    Items = x.Items,
                    Bins = x.Bins
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
                var result = await _saleOrderService
                    .GetSaleOrderPickQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode, true);

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
                            await _dialogService.DisplayAlert("Warning", "Sales order not found.", "OK");
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

        public static event Action<IEnumerable<SaleOrderPickQueueLookupViewModel>> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public PickQueueScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            ISaleOrderService saleOrderService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _saleOrderService = saleOrderService;
            _barcodeService = barcodeService;
        }
    }
}