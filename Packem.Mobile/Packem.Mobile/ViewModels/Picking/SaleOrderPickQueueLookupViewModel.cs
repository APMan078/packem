using Packem.Domain.Common.Enums;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Picking
{
    public class SaleOrderPickQueueLookupViewModel : ObservableObject
    {
        private int _saleOrderId;
        private string _saleOrderNo;
        private PickingStatusEnum _pickingStatus;
        private int _items;
        private int _bins;

        public int SaleOrderId
        {
            get => _saleOrderId;
            set { SetProperty(ref _saleOrderId, value); }
        }

        public string SaleOrderNo
        {
            get => _saleOrderNo;
            set { SetProperty(ref _saleOrderNo, value); }
        }

        public PickingStatusEnum PickingStatus
        {
            get => _pickingStatus;
            set { SetProperty(ref _pickingStatus, value); }
        }

        public int Items
        {
            get => _items;
            set { SetProperty(ref _items, value); }
        }

        public int Bins
        {
            get => _bins;
            set { SetProperty(ref _bins, value); }
        }
    }
}