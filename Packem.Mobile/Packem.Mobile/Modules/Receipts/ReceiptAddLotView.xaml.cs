using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Receipts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReceiptAddLotView : ContentPage
    {
        public ReceiptAddLotView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<ReceiptAddLotViewModel>();
        }
    }
}