using Packem.Mobile.Common.Base;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.KitchenSink.Syncfusion
{
    public class KitchenSinkSyncfusionViewModel : BaseViewModel
    {
        #region "Variables"

        private bool _popupOpen;

        #endregion

        #region "Properties"

        public bool PopupOpen
        {
            get => _popupOpen;
            set { SetProperty(ref _popupOpen, value); }
        }

        public class InventoryViewModel : BaseViewModel
        {
            private int _inventoryId;
            private string _location;
            private string _description;
            private int _available;

            public int InventoryId
            {
                get => _inventoryId;
                set { SetProperty(ref _inventoryId, value); }
            }

            public string Location
            {
                get => _location;
                set { SetProperty(ref _location, value); }
            }

            public string Description
            {
                get => _description;
                set { SetProperty(ref _description, value); }
            }

            public int Available
            {
                get => _available;
                set { SetProperty(ref _available, value); }
            }
        }

        public ObservableRangeCollection<InventoryViewModel> Inventories { get; }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand PopupCommand { get => new Command(async () => await Popup()); }

        #endregion

        #region "Functions"

        private async Task Appearing()
        {
            if (!Initialized)
            {
                await InitializeAsync();
                Initialized = true;
            }
        }

        private async Task Popup()
        {
            PopupOpen = true;

            await Task.FromResult(false);
        }

        #endregion

        public KitchenSinkSyncfusionViewModel()
        {
            Inventories = new ObservableRangeCollection<InventoryViewModel>();
            BindingBase.EnableCollectionSynchronization(Inventories, null, ObservableCollectionCallback);
        }

        void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            // `lock` ensures that only one thread access the collection at a time
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        public override Task InitializeAsync()
        {
            Inventories.Add(new InventoryViewModel
            {
                InventoryId = 1,
                Location = "BB",
                Description ="Salerno Fence 8x8",
                Available = 10000
            });
            Inventories.Add(new InventoryViewModel
            {
                InventoryId = 2,
                Location = "BB",
                Description = "AAA",
                Available = 5000
            });
            Inventories.Add(new InventoryViewModel
            {
                InventoryId = 3,
                Location = "AA",
                Description = "Sample description",
                Available = 100
            });
            Inventories.Add(new InventoryViewModel
            {
                InventoryId = 4,
                Location = "CC",
                Description = "CC",
                Available = 100000
            });
            Inventories.Add(new InventoryViewModel
            {
                InventoryId = 5,
                Location = "BB",
                Description = "Salerno Fence 8x8",
                Available = 10000
            });

            return Task.CompletedTask;
        }
    }
}
