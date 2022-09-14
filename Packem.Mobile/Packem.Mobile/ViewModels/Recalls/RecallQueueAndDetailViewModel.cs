using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Recalls
{
    public class RecallQueueAndDetailViewModel : ObservableObject
    {
        private RecallQueueLookupViewModel _recall;
        private RecallDetailViewModel _recallDetail;

        public RecallQueueLookupViewModel Recall
        {
            get => _recall;
            set { SetProperty(ref _recall, value); }
        }

        public RecallDetailViewModel RecallDetail
        {
            get => _recallDetail;
            set { SetProperty(ref _recallDetail, value); }
        }
    }
}