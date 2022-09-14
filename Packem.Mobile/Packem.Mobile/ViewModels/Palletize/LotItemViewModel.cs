using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Palletize
{
    public class LotItemViewModel : ObservableObject
    {
        private int _lotId;
        private int _itemId;
        private string _lotNo;
        private DateTime? _expirationDate;

        public int LotId
        {
            get => _lotId;
            set { SetProperty(ref _lotId, value); }
        }

        public int ItemId
        {
            get => _itemId;
            set { SetProperty(ref _itemId, value); }
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