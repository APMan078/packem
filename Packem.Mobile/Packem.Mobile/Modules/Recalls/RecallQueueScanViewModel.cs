using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Recalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Recalls
{
    public class RecallQueueScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IRecallService _recallService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<IEnumerable<RecallQueueLookupViewModel>> _scanResultEventManager =
            new WeakEventManager<IEnumerable<RecallQueueLookupViewModel>>();

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

        private IEnumerable<RecallQueueLookupViewModel> DtoToVm(IEnumerable<RecallQueueLookupGetModel> dto)
        {
            var vm = new List<RecallQueueLookupViewModel>();

            foreach (var x in dto)
            {
                vm.Add(new RecallQueueLookupViewModel
                {
                    RecallId = x.RecallId,
                    RecallDate = x.RecallDate,
                    Status = x.Status,
                    ItemSKU = x.ItemSKU,
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
                var result = await _recallService
                    .GetRecallQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, Barcode, true);

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

        public static event Action<IEnumerable<RecallQueueLookupViewModel>> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public RecallQueueScanViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IRecallService recallService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _recallService = recallService;
            _barcodeService = barcodeService;
        }
    }
}