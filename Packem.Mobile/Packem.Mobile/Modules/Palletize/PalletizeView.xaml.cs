using Autofac;
using Packem.Mobile.Modules.PutAways;
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
    public partial class PalletizeView : ContentPage
    {
        public PalletizeView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<PalletizeViewModel>();
        }
    }
}