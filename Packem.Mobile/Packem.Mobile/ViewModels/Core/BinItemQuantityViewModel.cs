using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Core
{
    public class BinItemQuantityViewModel : ObservableObject
    {
        private int _binId;
        private string _name;
        private int _qty;
        private string _lotNo;
        private DateTime? _expirationDate;

        public int BinId
        {
            get => _binId;
            set { SetProperty(ref _binId, value); }
        }

        public string Name
        {
            get => _name;
            set { SetProperty(ref _name, value); }
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