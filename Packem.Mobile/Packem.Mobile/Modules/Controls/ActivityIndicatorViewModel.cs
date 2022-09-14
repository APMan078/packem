using Packem.Mobile.Models.Enums;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.Modules.Controls
{
    public class ActivityIndicatorViewModel : ObservableObject
    {
        private bool isRunning;
        private ActivityIndicatorType activityIndicatorType;
        private string message;

        public bool IsRunning
        {
            get => isRunning;
            private set => SetProperty(ref isRunning, value);
        }

        public ActivityIndicatorType ActivityIndicatorType
        {
            get => activityIndicatorType;
            private set => SetProperty(ref activityIndicatorType, value);
        }

        public string Message
        {
            get => message;
            private set => SetProperty(ref message, value);
        }

        private ActivityIndicatorViewModel(bool isRunning, ActivityIndicatorType activityIndicatorType)
        {
            IsRunning = isRunning;
            ActivityIndicatorType = activityIndicatorType;
        }

        private ActivityIndicatorViewModel(bool isRunning, ActivityIndicatorType activityIndicatorType, string message)
        {
            IsRunning = isRunning;
            ActivityIndicatorType = activityIndicatorType;
            Message = message;
        }

        public ActivityIndicatorViewModel() {}

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelDefaultAndRunning()
            => new ActivityIndicatorViewModel(true, ActivityIndicatorType.Default);

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelDefaultAndNotRunning()
            => new ActivityIndicatorViewModel(false, ActivityIndicatorType.Default);

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelTransparentBackgroundAndRunning()
            => new ActivityIndicatorViewModel(true, ActivityIndicatorType.TransparentBackground);

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelTransparentBackgroundAndNotRunning()
            => new ActivityIndicatorViewModel(false, ActivityIndicatorType.TransparentBackground);

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelDefaultAndRunningWithMessage(string message)
            => new ActivityIndicatorViewModel(true, ActivityIndicatorType.DefaultWithMessage, message);

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelDefaultAndNotRunningWithMessage(string message)
            => new ActivityIndicatorViewModel(false, ActivityIndicatorType.DefaultWithMessage, message);

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelTransparentBackgroundAndRunningWithMessage(string message)
            => new ActivityIndicatorViewModel(true, ActivityIndicatorType.TransparentBackgroundWithMessage, message);

        public static ActivityIndicatorViewModel CreateActivityIndicatorViewModelTransparentBackgroundAndNotRunningWithMessage(string message)
            => new ActivityIndicatorViewModel(false, ActivityIndicatorType.TransparentBackgroundWithMessage, message);
    }
}
