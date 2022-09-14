using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Core
{
    public class BinOptionalPalletViewModel : ObservableObject
    {
        private int _binId;
        private string _name;
        private int? _palletCount;
        private int? _eachCount;
        private int? _palletQty;
        private int? _eachQty;
        private int? _totalQty;

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

        public int? PalletCount
        {
            get => _palletCount;
            set { SetProperty(ref _palletCount, value); }
        }

        public int? EachCount
        {
            get => _eachCount;
            set { SetProperty(ref _eachCount, value); }
        }

        public int? PalletQty
        {
            get => _palletQty;
            set { SetProperty(ref _palletQty, value); }
        }

        public int? EachQty
        {
            get => _eachQty;
            set { SetProperty(ref _eachQty, value); }
        }

        public int? TotalQty
        {
            get => _totalQty;
            set { SetProperty(ref _totalQty, value); }
        }
    }
}