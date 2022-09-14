using Packem.Domain.Common.Enums;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Palletize
{
    public class LicensePlateViewModel : ObservableObject
    {
        private int _licensePlateId;
        private string _licensePlateNo;
        private LicensePlateTypeEnum _licensePlateType;

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
    }
}