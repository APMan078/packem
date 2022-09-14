using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.PutAways
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PutAwayBinLookupPalletView : ContentPage
    {
        public PutAwayBinLookupPalletView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<PutAwayBinLookupPalletViewModel>();
        }
    }
}