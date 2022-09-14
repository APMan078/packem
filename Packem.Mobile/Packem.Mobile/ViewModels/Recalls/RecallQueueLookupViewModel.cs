using Packem.Domain.Common.Enums;
using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Recalls
{
    public class RecallQueueLookupViewModel : ObservableObject
    {
        private int _recallId;
        private DateTime _recallDate;
        private RecallStatusEnum _status;
        private string _itemSKU;
        private string _itemDescription;
        private string _itemUOM;
        private int _bins;

        public int RecallId
        {
            get => _recallId;
            set { SetProperty(ref _recallId, value); }
        }

        public DateTime RecallDate
        {
            get => _recallDate;
            set { SetProperty(ref _recallDate, value); }
        }

        public RecallStatusEnum Status
        {
            get => _status;
            set { SetProperty(ref _status, value); }
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

        public string ItemUOM
        {
            get => _itemUOM;
            set { SetProperty(ref _itemUOM, value); }
        }

        public int Bins
        {
            get => _bins;
            set { SetProperty(ref _bins, value); }
        }
    }
}