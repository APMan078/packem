using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Base
{
    public class BarcodeScanBaseViewModel : BaseViewModel
    {
        #region "Variables"

        private string _barcode;
        private bool _isTorchOn;

        #endregion

        #region "Properties"

        public string Barcode
        {
            get => _barcode;
            set { SetProperty(ref _barcode, value); }
        }

        public bool IsTorchOn
        {
            get => _isTorchOn;
            set { SetProperty(ref _isTorchOn, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand FlashCommand
            => new Command(Flash,
                canExecute: () => IsNotBusy);

        #endregion

        #region "Functions"

        private async Task Appearing()
        {
            if (!Initialized)
            {
                await InitializeAsync();
                Initialized = true;
            }
        }

        private void Flash()
        {
            try
            {
                IsBusy = true;

                IsTorchOn = !IsTorchOn;
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;

                await Task.Delay(1000)
                    .ContinueWith((t) =>
                    {
                        MainThread.BeginInvokeOnMainThread(() => IsTorchOn = true);
                    });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}