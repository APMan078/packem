using Xamarin.CommunityToolkit.ObjectModel;

namespace Packem.Mobile.ViewModels.Core
{
    public class BinViewModel : ObservableObject
    {
        private int _binId;
        private string _name;

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
    }
}
