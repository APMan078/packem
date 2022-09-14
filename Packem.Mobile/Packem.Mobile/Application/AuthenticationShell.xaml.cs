using Packem.Mobile.Models;
using Packem.Mobile.Modules.Authentications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Application
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthenticationShell : Shell
    {
        public AuthenticationShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("RegisterViewModel/RegisterDeviceViewModel", typeof(RegisterDeviceView));
            Routing.RegisterRoute("RegisteredDeviceViewModel/LoginViewModel", typeof(LoginView));
        }
    }
}