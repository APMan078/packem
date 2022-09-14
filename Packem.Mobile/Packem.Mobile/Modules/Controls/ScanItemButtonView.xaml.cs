using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanItemButtonView : StackLayout
    {
        public static readonly BindableProperty ScanItemCommandProperty =
            BindableProperty.Create("ScanItemCommand", typeof(ICommand), typeof(ScanItemButtonView), null);

        public ICommand ScanItemCommand
        {
            get { return (ICommand)GetValue(ScanItemCommandProperty); }
            set { SetValue(ScanItemCommandProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(ScanItemButtonView), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ScanItemButtonView()
        {
            InitializeComponent();

            buttonScanItem.SetBinding(SfButton.CommandProperty, new Binding("ScanItemCommand", source: this));
            buttonScanItem.SetBinding(SfButton.TextProperty, new Binding("Text", source: this));
        }
    }
}