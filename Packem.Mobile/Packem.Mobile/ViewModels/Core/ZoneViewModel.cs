using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Core
{
    public class ZoneViewModel : ObservableObject
    {
        private int _zoneId;
        private string _name;

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
    }
}