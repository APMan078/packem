using Packem.Mobile.ViewModels.Core;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Picking
{
    public class OrderLinePickLookupViewModel : ObservableObject
    {
        private int _orderLineId;
        private int _itemId;
        private string _itemSKU;
        private string _itemDescription;
        private string _itemUOM;
        private int _qty;
        private int _received;
        private int _remaining;
        private bool _completed;

        public int OrderLineId
        {
            get => _orderLineId;
            set { SetProperty(ref _orderLineId, value); }
        }

        public int ItemId
        {
            get => _itemId;
            set { SetProperty(ref _itemId, value); }
        }

        public string ItemSKU
        {
            get => _itemSKU;
            set { SetProperty(ref _itemSKU, value); }
        }

        public string ItemDescription
        {
            get => _itemDescription;
            set { SetProperty(ref _itemDescription, value); }
        }

        public string ItemUOM
        {
            get => _itemUOM;
            set { SetProperty(ref _itemUOM, value); }
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

        public bool Completed
        {
            get => _completed;
            set { SetProperty(ref _completed, value); }
        }

        public ObservableRangeCollection<BinViewModel> Bins { get; }

        public OrderLinePickLookupViewModel()
        {
            Bins = new ObservableRangeCollection<BinViewModel>();
        }
    }
}