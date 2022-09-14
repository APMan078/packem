using Packem.Mobile.Modules.Controls;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;

namespace Packem.Mobile.Common.Base
{
    public abstract class BaseViewModel : ObservableObject
    {
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    IsNotBusy = !_isBusy;
                }
            }
        }

        private bool _isNotBusy = true;
        public bool IsNotBusy
        {
            get => _isNotBusy;
            set
            {
                if (SetProperty(ref _isNotBusy, value))
                {
                    IsBusy = !_isNotBusy;
                }
            }
        }

        private bool _initialized;
        public bool Initialized
        {
            get => _initialized;
            set => SetProperty(ref _initialized, value);
        }

        private LayoutState _currentState;
        public LayoutState CurrentState
        {
            get => _currentState;
            set => SetProperty(ref _currentState, value);
        }

        private ActivityIndicatorViewModel activityIndicatorViewModel
            = ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

        public ActivityIndicatorViewModel ActivityIndicatorViewModel
        {
            get => activityIndicatorViewModel;
            set => SetProperty(ref activityIndicatorViewModel, value);
        }
    }
}
