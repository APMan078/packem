using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Inventories
{
    public class InventoryLocationViewModel : ObservableObject
    {
        private int _locationId;
        private string _location;
        private string _description;
        private string _uom;
        private int _available;

        public int LocationId
        {
            get => _locationId;
            set { SetProperty(ref _locationId, value); }
        }

        public string Location
        {
            get => _location;
            set { SetProperty(ref _location, value); }
        }

        public string UOM
        {
            get => _uom;
            set { SetProperty(ref _uom, value); }
        }

        public string Description
        {
            get => _description;
            set { SetProperty(ref _description, value); }
        }

        public int Available
        {
            get => _available;
            set { SetProperty(ref _available, value); }
        }
    }
}
