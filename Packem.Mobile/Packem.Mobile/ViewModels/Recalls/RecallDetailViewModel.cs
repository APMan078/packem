using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Recalls
{
    public class RecallDetailViewModel : ObservableObject
    {
        private int _zoneId;
        private string _zoneName;
        private int _binId;
        private string _binName;
        private int _qty;
        private int _received;
        private int _remaining;

        public int ZoneId
        {
            get => _zoneId;
            set { SetProperty(ref _zoneId, value); }
        }

        public string ZoneName
        {
            get => _zoneName;
            set { SetProperty(ref _zoneName, value); }
        }

        public int BinId
        {
            get => _binId;
            set { SetProperty(ref _binId, value); }
        }

        public string BinName
        {
            get => _binName;
            set { SetProperty(ref _binName, value); }
        }

        public int Qty
        {
            get => _qty;
            set { SetProperty(ref _qty, value); }
        }

        public int Received
        {
            get => _received;
            set { SetProperty(ref _received, value); }
        }

        public int Remaining
        {
            get => _remaining;
            set { SetProperty(ref _remaining, value); }
        }
    }
}