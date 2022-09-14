using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace Packem.Mobile.Modules.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeScannerView : StackLayout
    {
        public static readonly BindableProperty ScanResultCommandProperty =
            BindableProperty.Create("ScanResultCommand", typeof(ICommand), typeof(BarcodeScannerView), null);

        public ICommand ScanResultCommand
        {
            get { return (ICommand)GetValue(ScanResultCommandProperty); }
            set { SetValue(ScanResultCommandProperty, value); }
        }

        public static readonly BindableProperty FlashCommandProperty =
            BindableProperty.Create("FlashCommand", typeof(ICommand), typeof(BarcodeScannerView), null);

        public ICommand FlashCommand
        {
            get { return (ICommand)GetValue(FlashCommandProperty); }
            set { SetValue(FlashCommandProperty, value); }
        }

        public static readonly BindableProperty IsAnalyzingProperty =
            BindableProperty.Create("IsAnalyzing", typeof(bool), typeof(BarcodeScannerView), null);

        public bool IsAnalyzing
        {
            get { return (bool)GetValue(IsAnalyzingProperty); }
            set { SetValue(IsAnalyzingProperty, value); }
        }

        public static readonly BindableProperty IsScanningProperty =
            BindableProperty.Create("IsScanning", typeof(bool), typeof(BarcodeScannerView), null);

        public bool IsScanning
        {
            get { return (bool)GetValue(IsScanningProperty); }
            set { SetValue(IsScanningProperty, value); }
        }

        public static readonly BindableProperty IsTorchOnProperty =
            BindableProperty.Create("IsTorchOn", typeof(bool), typeof(BarcodeScannerView), null);

        public bool IsTorchOn
        {
            get { return (bool)GetValue(IsTorchOnProperty); }
            set { SetValue(IsTorchOnProperty, value); }
        }

        public BarcodeScannerView()
        {
            InitializeComponent();

            scanner.SetBinding(ZXingScannerView.ScanResultCommandProperty, new Binding("ScanResultCommand", source: this));
            scannerOverlay.SetBinding(ZXingDefaultOverlay.FlashCommandProperty, new Binding("FlashCommand", source: this));
            scanner.SetBinding(ZXingScannerView.IsAnalyzingProperty, new Binding("IsAnalyzing", source: this));
            scanner.SetBinding(ZXingScannerView.IsScanningProperty, new Binding("IsScanning", source: this));
            scanner.SetBinding(ZXingScannerView.IsTorchOnProperty, new Binding("IsTorchOn", source: this));
        }
    }
}