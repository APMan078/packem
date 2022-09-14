using Packem.Mobile.Common.DependencyServices;
using Packem.Mobile.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(PrintService))]
namespace Packem.Mobile.iOS.DependencyServices
{
    public class PrintService : IPrintService
    {
        public void Print(WebView viewToPrint)
        {
            var appleViewToPrint = Platform.CreateRenderer(viewToPrint).NativeView;

            var printInfo = UIPrintInfo.PrintInfo;

            printInfo.OutputType = UIPrintInfoOutputType.General;
            printInfo.JobName = "Packem Barcode Print";
            printInfo.Orientation = UIPrintInfoOrientation.Portrait;
            printInfo.Duplex = UIPrintInfoDuplex.None;

            var printController = UIPrintInteractionController.SharedPrintController;

            printController.PrintInfo = printInfo;
            printController.ShowsPageRange = true;
            printController.PrintFormatter = appleViewToPrint.ViewPrintFormatter;

            printController.Present(true, (printInteractionController, completed, error) => { });
        }
    }
}