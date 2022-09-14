using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Authentications
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisteredDeviceView : ContentPage
    {
        public RegisteredDeviceView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<RegisteredDeviceViewModel>();
        }
    }
}