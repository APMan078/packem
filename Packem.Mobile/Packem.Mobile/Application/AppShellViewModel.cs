using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.Modules.Authentications;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Application
{
    public class AppShellViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public AppShellViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public ICommand SignOutCommand { get => new Command(async () => await SignOut()); }

        private async Task SignOut()
        {
            Preferences.Remove(Constants.IS_USER_LOGGED_IN);
            _navigationService.GoToLoginFlow();
            //await _navigationService.InsertAsRoot<LoginViewModel>();
            await _navigationService.InsertAsRoot<RegisterViewModel>();
        }
    }
}
