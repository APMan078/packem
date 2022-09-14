using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Inventories
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryBarcodeScanView : ContentPage
    {
        public InventoryBarcodeScanView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<InventoryBarcodeScanViewModel>();
        }
    }
}