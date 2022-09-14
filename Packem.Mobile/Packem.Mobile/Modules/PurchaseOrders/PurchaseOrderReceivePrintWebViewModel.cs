using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.PurchaseOrders
{
    [QueryProperty("SKU", "sku")]
    [QueryProperty("Desciption", "desc")]
    public class PurchaseOrderReceivePrintWebViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly IDialogService _dialogService;

        private string sku;
        private string desciption;
        private string html;

        #endregion

        #region "Properties"

        public string SKU
        {
            get => sku;
            set => SetProperty(ref sku, value);
        }

        public string Desciption
        {
            get => desciption;
            set => SetProperty(ref desciption, value);
        }

        public string Html
        {
            get => html;
            set => SetProperty(ref html, value);
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

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

        #endregion

        public PurchaseOrderReceivePrintWebViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;

                var width = 400; // width of the Qr Code
                var height = 150; // height of the Qr Code
                var margin = 0;
                var qrCodeWriter = new ZXing.BarcodeWriterPixelData
                {
                    Format = ZXing.BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = margin
                    }
                };
                var pixelData = qrCodeWriter.Write(SKU);

                var base64String = string.Empty;
                using (var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixelData.Pixels, width, height))
                {
                    base64String = image.ToBase64String(PngFormat.Instance);
                }

                Html = $@"
                    <html>
                        <body>
                          <div style='padding: 20px; max-width: 500px; height: 300px;'>
                            <h4 style='text-align: center;'>{Desciption}</h4>
                            <h4 style='text-align: center;'>SKU {SKU}</h4>
                            <div>
                                <img src = '{base64String}' style='margin-left: auto; margin-right: auto; display: block; width: 100%; height: auto; object-fit: cover;' />
                            </div>
                          </div>
                        </body>
                    </html>";

                if (Forms9Patch.PrintService.CanPrint)
                {
                    await Forms9Patch.PrintService.PrintAsync(Html, "Barcode Print", Forms9Patch.FailAction.ShowAlert);

                    //try
                    //{
                    //    var printService = DependencyService.Get<IPrintService>();
                    //    printService.Print(printWebView);
                    //}
                    //catch (Exception ex)
                    //{
                    //    await Shell.Current.DisplayAlert("a", ex.ToString(), "ok");
                    //}
                }
                else
                {
                    await _dialogService.DisplayAlert("Error", "No printer found.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
