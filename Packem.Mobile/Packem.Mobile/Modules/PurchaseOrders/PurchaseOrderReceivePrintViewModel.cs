using Packem.Mobile.Common.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;

namespace Packem.Mobile.Modules.PurchaseOrders
{
    public class PurchaseOrderReceivePrintViewModel : BaseViewModel
    {
        #region "Variables"
        #endregion

        #region "Properties"
        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        #endregion

        #region "Functions"

        private async Task Appearing()
        {
            if (!Initialized)
            {
                await InitializeAsync();
                Initialized = true;
            }
        }

        #endregion

        public override Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                CurrentState = LayoutState.Loading;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();
            }
            finally
            {
                IsBusy = false;
                CurrentState = LayoutState.Success;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }

            return Task.CompletedTask;
        }
    }
}
