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
    public partial class PalletizeAddLotView : ContentPage
    {
        public PalletizeAddLotView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<PalletizeAddLotViewModel>();
        }
    }
}