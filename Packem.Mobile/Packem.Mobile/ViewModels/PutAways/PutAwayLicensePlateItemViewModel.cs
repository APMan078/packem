using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.PutAways
{
    public class PutAwayLicensePlateItemViewModel : ObservableObject
    {
        private int _itemId;
        private string _sku;
        private string _description;
        private int _totalQty;

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

        public int TotalQty
        {
            get => _totalQty;
            set { SetProperty(ref _totalQty, value); }
        }
    }
}