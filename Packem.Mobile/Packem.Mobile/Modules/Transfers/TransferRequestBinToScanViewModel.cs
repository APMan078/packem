using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.Modules.Transfers
{
    public class TransferRequestBinToScanViewModel : BarcodeScanBaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IBarcodeService _barcodeService;
        private static readonly WeakEventManager<string> _scanResultEventManager =
            new WeakEventManager<string>();

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

        private async Task BarcodeScanResult(ZXing.Result r)
        {
            try
            {
                IsBusy = true;

                Barcode = r.Text;
                _barcodeService.PlaySound();

                _scanResultEventManager.RaiseEvent(Barcode, nameof(ScanResult));
                await _navigationService.PopAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<string> ScanResult
        {
            add => _scanResultEventManager.AddEventHandler(value);
            remove => _scanResultEventManager.RemoveEventHandler(value);
        }

        #endregion

        public TransferRequestBinToScanViewModel(INavigationService navigationService,
            IBarcodeService barcodeService)
        {
            _navigationService = navigationService;
            _barcodeService = barcodeService;
        }
    }
}