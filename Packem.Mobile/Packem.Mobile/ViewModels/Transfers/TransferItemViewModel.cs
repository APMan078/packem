using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Transfers
{
    public class TransferItemViewModel : ObservableObject
    {
        private int _transferId;
        private int _itemId;
        private string _itemSku;
        private string _itemDescription;
        private string _itemUom;
        private string _currentZone;
        private int _currentZoneId;
        private string _currentBin;
        private int _currentBinId;
        private int _currentBinQty;
        private string _newZone;
        private int? _newZoneId;
        private string _newBin;
        private int? _newBinId;
        private int _qtyToTransfer;
        private int _remaining;
        private string _lotNo;
        private DateTime? _expirationDate;

        public int TransferId
        {
            get => _transferId;
            set { SetProperty(ref _transferId, value); }
        }

        public int ItemId
        {
            get => _itemId;
            set { SetProperty(ref _itemId, value); }
        }

        public string ItemSKU
        {
            get => _itemSku;
            set { SetProperty(ref _itemSku, value); }
        }

        public string ItemDescription
        {
            get => _itemDescription;
            set { SetProperty(ref _itemDescription, value); }
        }

        public string ItemUOM
        {
            get => _itemUom;
            set { SetProperty(ref _itemUom, value); }
        }

        public string CurrentZone
        {
            get => _currentZone;
            set { SetProperty(ref _currentZone, value); }
        }

        public int CurrentZoneId
        {
            get => _currentZoneId;
            set { SetProperty(ref _currentZoneId, value); }
        }

        public string CurrentBin
        {
            get => _currentBin;
            set { SetProperty(ref _currentBin, value); }
        }

        public int CurrentBinId
        {
            get => _currentBinId;
            set { SetProperty(ref _currentBinId, value); }

        }
        public int CurrentBinQty
        {
            get => _currentBinQty;
            set { SetProperty(ref _currentBinQty, value); }
        }

        public string NewZone
        {
            get => _newZone;
            set { SetProperty(ref _newZone, value); }
        }

        public int? NewZoneId
        {
            get => _newZoneId;
            set { SetProperty(ref _newZoneId, value); }
        }

        public string NewBin
        {
            get => _newBin;
            set { SetProperty(ref _newBin, value); }
        }

        public int? NewBinId
        {
            get => _newBinId;
            set { SetProperty(ref _newBinId, value); }
        }

        public int QtyToTransfer
        {
            get => _qtyToTransfer;
            set { SetProperty(ref _qtyToTransfer, value); }
        }

        public int Remaining
        {
            get => _remaining;
            set { SetProperty(ref _remaining, value); }
        }

        public string LotNo
        {
            get => _lotNo;
            set { SetProperty(ref _lotNo, value); }
        }

        public DateTime? ExpirationDate
        {
            get => _expirationDate;
            set { SetProperty(ref _expirationDate, value); }
        }
    }
}