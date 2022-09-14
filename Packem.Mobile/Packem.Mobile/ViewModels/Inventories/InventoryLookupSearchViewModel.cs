using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Inventories
{
    public class InventoryLookupSearchViewModel : ObservableObject
    {
        public ItemViewModel Item { get; set; }
        public ObservableRangeCollection<BinZoneViewModel> BinZones { get; }

        public InventoryLookupSearchViewModel()
        {
            BinZones = new ObservableRangeCollection<BinZoneViewModel>();
        }
    }
}