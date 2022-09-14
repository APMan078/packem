using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Inventories
{
    public class ItemViewModel : ObservableObject
    {
        private int _itemId;
        private string _sku;
        private string _description;
        private string _uom;
        private int _qtyOnHand;
        private int _onOrder;

        public int ItemId
        {
            get => _itemId;
            set { SetProperty(ref _itemId, value); }
        }

        public string SKU
        {
            get => _sku;
            set { SetProperty(ref _sku, value); }
        }

        public string Description
        {
            get => _description;
            set { SetProperty(ref _description, value); }
        }

        public string UOM
        {
            get => _uom;
            set { SetProperty(ref _uom, value); }
        }

        public int QtyOnHand
        {
            get => _qtyOnHand;
            set { SetProperty(ref _qtyOnHand, value); }
        }

        public int OnOrder
        {
            get => _onOrder;
            set { SetProperty(ref _onOrder, value); }
        }
    }
}
