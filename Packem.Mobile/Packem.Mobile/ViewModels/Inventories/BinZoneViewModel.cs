using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Inventories
{
    public class BinZoneViewModel : ObservableObject
    {
        private int _zoneId;
        private string _zone;
        private int _binId;
        private string _bin;
        private int _qty;
        private string _lotNo;
        private DateTime? _expirationDate;

        public int ZoneId
        {
            get => _zoneId;
            set { SetProperty(ref _zoneId, value); }
        }

        public string Zone
        {
            get => _zone;
            set { SetProperty(ref _zone, value); }
        }

        public int BinId
        {
            get => _binId;
            set { SetProperty(ref _binId, value); }
        }

        public string Bin
        {
            get => _bin;
            set { SetProperty(ref _bin, value); }
        }

        public int Qty
        {
            get => _qty;
            set { SetProperty(ref _qty, value); }
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