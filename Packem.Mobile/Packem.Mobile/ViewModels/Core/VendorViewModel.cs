using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Core
{
    public class VendorViewModel : ObservableObject
    {
        private int _vendorId;
        private string _name;

        public int VendorId
        {
            get => _vendorId;
            set { SetProperty(ref _vendorId, value); }
        }

        public string Name
        {
            get => _name;
            set { SetProperty(ref _name, value); }
        }
    }
}