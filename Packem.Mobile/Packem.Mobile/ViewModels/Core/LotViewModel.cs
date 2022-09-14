using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Core
{
    public class LotViewModel : ObservableObject
    {
        private int _lotId;
        private string _lotNo;
        private DateTime? _expirationDate;

        public int LotId
        {
            get => _lotId;
            set { SetProperty(ref _lotId, value); }
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