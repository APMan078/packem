using Packem.Mobile.ViewModels.Core;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Palletize
{
    public class PalletizeItemViewModel : ObservableObject
    {
        private int? _licensePlateItemId;
        private int _itemId;
        private string _itemSKU;
        private string _itemDescription;
        private int? _vendorId;
        private string _vendorName;
        private int? _lotId;
        private string _lotNo;
        private string _referenceNo;
        private int? _cases;
        private int? _eacase;
        private int? _totalQty;
        private int? _totalWgt;
        private LotViewModel _lot;
        private VendorViewModel _vendor;

        public int? LicensePlateItemId
        {
            get => _licensePlateItemId;
            set { SetProperty(ref _licensePlateItemId, value); }
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

        public int? VendorId
        {
            get => _vendorId;
            set { SetProperty(ref _vendorId, value); }
        }

        public string VendorName
        {
            get => _vendorName;
            set { SetProperty(ref _vendorName, value); }
        }

        public int? LotId
        {
            get => _lotId;
            set { SetProperty(ref _lotId, value); }
        }

        public string LotNo
        {
            get => _lotNo;
            set { SetProperty(ref _lotNo, value); }
        }

        public string ReferenceNo
        {
            get => _referenceNo;
            set { SetProperty(ref _referenceNo, value); }
        }

        public int? Cases
        {
            get => _cases;
            set { SetProperty(ref _cases, value); }
        }

        public int? EACase
        {
            get => _eacase;
            set { SetProperty(ref _eacase, value); }
        }

        public int? TotalQty
        {
            get => _totalQty;
            set { SetProperty(ref _totalQty, value); }
        }

        public int? TotalWgt
        {
            get => _totalWgt;
            set { SetProperty(ref _totalWgt, value); }
        }

        public LotViewModel Lot
        {
            get => _lot;
            set { SetProperty(ref _lot, value); }
        }

        public VendorViewModel Vendor
        {
            get => _vendor;
            set { SetProperty(ref _vendor, value); }
        }

        public ObservableRangeCollection<LotViewModel> Lots { get; }
        public ObservableRangeCollection<VendorViewModel> Vendors { get; }

        public PalletizeItemViewModel()
        {
            Lots = new ObservableRangeCollection<LotViewModel>();
            Vendors = new ObservableRangeCollection<VendorViewModel>();
        }
    }
}