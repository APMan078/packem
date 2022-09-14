using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.PurchaseOrders
{
    public class PurchaseOrderViewModel : ObservableObject
    {
        private int _purchaseOrderId;
        private string _purchaseOrderNo;
        private string _vendorNo;
        private string _vendorName;

        public int PurchaseOrderId
        {
            get => _purchaseOrderId;
            set { SetProperty(ref _purchaseOrderId, value); }
        }

        public string PurchaseOrderNo
        {
            get => _purchaseOrderNo;
            set { SetProperty(ref _purchaseOrderNo, value); }
        }

        public string VendorNo
        {
            get => _vendorNo;
            set { SetProperty(ref _vendorNo, value); }
        }

        public string VendorName
        {
            get => _vendorName;
            set { SetProperty(ref _vendorName, value); }
        }
    }
}
