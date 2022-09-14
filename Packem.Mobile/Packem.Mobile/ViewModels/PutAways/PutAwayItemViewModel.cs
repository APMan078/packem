using Packem.Domain.Common.Enums;
using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.PutAways
{
    public class PutAwayItemViewModel : ObservableObject
    {
        private int _putAwayId;
        private int _itemId;
        private string _sku;
        private string _description;
        private int _remaining;
        private PutAwayTypeEnum _putAwayType;
        private string _lotNo;
        private DateTime? _expirationDate;

        public int PutAwayId
        {
            get => _putAwayId;
            set { SetProperty(ref _putAwayId, value); }
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

        public int Remaining
        {
            get => _remaining;
            set { SetProperty(ref _remaining, value); }
        }

        public PutAwayTypeEnum PutAwayType
        {
            get => _putAwayType;
            set { SetProperty(ref _putAwayType, value); }
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