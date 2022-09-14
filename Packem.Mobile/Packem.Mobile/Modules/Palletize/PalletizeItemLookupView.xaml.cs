using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Palletize
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletizeItemLookupView : ContentPage
    {
        public PalletizeItemLookupView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<PalletizeItemLookupViewModel>();
        }
    }
}