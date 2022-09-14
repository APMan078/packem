using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Core
{
    public class ZoneItemQuantityViewModel : ObservableObject
    {
        private int _zoneId;
        private string _name;
        private int _qty;

        public int ZoneId
        {
            get => _zoneId;
            set { SetProperty(ref _zoneId, value); }
        }

        public string Name
        {
            get => _name;
            set { SetProperty(ref _name, value); }
        }

        public int Qty
        {
            get => _qty;
            set { SetProperty(ref _qty, value); }
        }
    }
}