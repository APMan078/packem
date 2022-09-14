using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.PurchaseOrders
{
    public class ReceiveViewModel : ObservableObject
    {
        private int _receiveId;
        private int _itemId;
        private string _sku;
        private string _description;
        private string _uom;
        private int _remaining;

        public int ReceiveId
        {
            get => _receiveId;
            set { SetProperty(ref _receiveId, value); }
        }

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

        public int Remaining
        {
            get => _remaining;
            set { SetProperty(ref _remaining, value); }
        }
    }
}
