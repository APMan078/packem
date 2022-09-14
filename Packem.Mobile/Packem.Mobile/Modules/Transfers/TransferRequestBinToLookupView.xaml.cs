using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Transfers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferRequestBinToLookupView : ContentPage
    {
        public TransferRequestBinToLookupView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<TransferRequestBinToLookupViewModel>();
        }
    }
}