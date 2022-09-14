using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.PurchaseOrders
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PurchaseOrderReceivePrintView : ContentPage
    {
        public PurchaseOrderReceivePrintView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<PurchaseOrderReceivePrintViewModel>();
        }
    }
}