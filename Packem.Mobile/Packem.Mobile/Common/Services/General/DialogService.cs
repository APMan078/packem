using Packem.Mobile.Common.Interfaces.General;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Services.General
{
    public class DialogService : IDialogService
    {
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await Shell.Current.DisplayAlert(title, message, cancel);
        }

        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return Shell.Current.DisplayAlert(title, message, accept, cancel);
        }

        public Task<string> DisplayPrompt(string title, string message, string accept, string cancel)
        {
            return Shell.Current.DisplayPromptAsync(title, message, accept, cancel);
        }

        public Task<string> DisplayActionSheet(string title, string destruction, params string[] buttons)
        {
            return Shell.Current.DisplayActionSheet(title, "Cancel", destruction, buttons);
        }
    }
}
