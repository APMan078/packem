using Packem.Mobile.ViewModels.Core;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Palletize
{
    public class ItemVendorViewModel : ObservableObject
    {
        private ItemViewModel _item;

        public ItemViewModel Item
        {
            get => _item;
            set { SetProperty(ref _item, value); }
        }

        public ObservableRangeCollection<LotViewModel> Lots { get; }
        public ObservableRangeCollection<VendorViewModel> Vendors { get; }

        public ItemVendorViewModel()
        {
            Lots = new ObservableRangeCollection<LotViewModel>();
            Vendors = new ObservableRangeCollection<VendorViewModel>();
        }
    }
}