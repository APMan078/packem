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
    public partial class RegisterDeviceView : ContentPage
    {
        public RegisterDeviceView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<RegisterDeviceViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var vm = BindingContext as RegisterDeviceViewModel;

            if (!vm.Initialized)
            {
                await vm.InitializeAsync();
                vm.Initialized = true;
            }
        }
    }
}