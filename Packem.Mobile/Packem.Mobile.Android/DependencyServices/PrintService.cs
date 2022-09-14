using Android.Content;
using Android.Print;
using Android.PrintServices;
using Packem.Mobile.Common.DependencyServices;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(PrintService))]
namespace Packem.Mobile.Droid.DependencyServices
{
    public class PrintService : IPrintService
    {
        Context _context = Android.App.Application.Context;

        public void Print(WebView viewToPrint)
        {
            IVisualElementRenderer existingRenderer = Platform.GetRenderer(viewToPrint);
            if ((existingRenderer ?? Platform.CreateRendererWithContext(viewToPrint, _context)) is IVisualElementRenderer renderer)
            {
                Android.Webkit.WebView droidWebView = renderer.View as Android.Webkit.WebView;
                if (droidWebView == null && renderer.View is WebViewRenderer xfWebViewRenderer)
                    droidWebView = xfWebViewRenderer.Control;
                if (droidWebView != null)
                {
                    // Only valid for API 19+
                    var version = Android.OS.Build.VERSION.SdkInt;

                    if (version >= Android.OS.BuildVersionCodes.Kitkat)
                    {
                        droidWebView.Settings.JavaScriptEnabled = true;
                        droidWebView.Settings.DomStorageEnabled = true;
                        droidWebView.SetLayerType(Android.Views.LayerType.Software, null);

                        var printMgr = (PrintManager)_context.GetSystemService(Context.PrintService);
                        printMgr.Print("Packem Barcode Print", droidWebView.CreatePrintDocumentAdapter("Packem Barcode Print"), null);
                    }
                }
            }
        }
    }
}
