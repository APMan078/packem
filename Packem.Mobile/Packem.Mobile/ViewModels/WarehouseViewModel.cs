using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels
{
    public class WarehouseViewModel : ObservableObject
    {
        private int _warehouseId;
        private string _name;

        public int WarehouseId
        {
            get => _warehouseId;
            set { SetProperty(ref _warehouseId, value); }
        }

        public string Name
        {
            get => _name;
            set { SetProperty(ref _name, value); }
        }
    }
}
