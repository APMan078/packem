using Syncfusion.XForms.Buttons;
using Syncfusion.XForms.TextInputLayout;
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
    public partial class EntryWithScanView : SfTextInputLayout
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(EntryWithScanAndSearchView), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty ScanCommandProperty =
            BindableProperty.Create("ScanCommand", typeof(ICommand), typeof(EntryWithScanAndSearchView), null);

        public ICommand ScanCommand
        {
            get { return (ICommand)GetValue(ScanCommandProperty); }
            set { SetValue(ScanCommandProperty, value); }
        }

        public static readonly BindableProperty UnfocusedCommandProperty =
            BindableProperty.Create("UnfocusedCommand", typeof(ICommand), typeof(EntryWithScanAndSearchView), null);

        public ICommand UnfocusedCommand
        {
            get { return (ICommand)GetValue(UnfocusedCommandProperty); }
            set { SetValue(UnfocusedCommandProperty, value); }
        }

        public static readonly BindableProperty UserStoppedTypingCommandProperty =
            BindableProperty.Create("UserStoppedTypingCommand", typeof(ICommand), typeof(EntryWithScanAndSearchView), null);

        public ICommand UserStoppedTypingCommand
        {
            get { return (ICommand)GetValue(UserStoppedTypingCommandProperty); }
            set { SetValue(UserStoppedTypingCommandProperty, value); }
        }

        public EntryWithScanView()
        {
            InitializeComponent();

            entryText.SetBinding(Entry.TextProperty, new Binding("Text", source: this));
            buttonScan.SetBinding(SfButton.CommandProperty, new Binding("ScanCommand", source: this));
        }
    }
}