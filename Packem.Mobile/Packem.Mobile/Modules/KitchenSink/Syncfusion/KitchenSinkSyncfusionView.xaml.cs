using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.KitchenSink.Syncfusion
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KitchenSinkSyncfusionView : ContentPage
    {
        public KitchenSinkSyncfusionView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<KitchenSinkSyncfusionViewModel>();
        }
    }
}