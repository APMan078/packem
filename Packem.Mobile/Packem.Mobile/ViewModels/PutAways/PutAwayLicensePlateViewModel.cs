using Packem.Domain.Common.Enums;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.PutAways
{
    public class PutAwayLicensePlateViewModel : ObservableObject
    {
        private int _putAwayId;
        private int _licensePlateId;
        private string _licensePlateNo;
        private LicensePlateTypeEnum _licensePlateType;
        private bool _showSeparator;

        public int PutAwayId
        {
            get => _putAwayId;
            set { SetProperty(ref _putAwayId, value); }
        }

        public int LicensePlateId
        {
            get => _licensePlateId;
            set { SetProperty(ref _licensePlateId, value); }
        }

        public string LicensePlateNo
        {
            get => _licensePlateNo;
            set { SetProperty(ref _licensePlateNo, value); }
        }

        public LicensePlateTypeEnum LicensePlateType
        {
            get => _licensePlateType;
            set { SetProperty(ref _licensePlateType, value); }
        }

        public bool ShowSeparator
        {
            get => _showSeparator;
            set { SetProperty(ref _showSeparator, value); }
        }

        public ObservableRangeCollection<PutAwayLicensePlateItemViewModel> Items { get; }

        public PutAwayLicensePlateViewModel()
        {
            Items = new ObservableRangeCollection<PutAwayLicensePlateItemViewModel>();
        }
    }
}