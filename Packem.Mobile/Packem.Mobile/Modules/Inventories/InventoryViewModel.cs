using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Modules.Barcode;
using Packem.Mobile.ViewModels.Inventories;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Inventories
{
    public class InventoryViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;

        private InventoryLookupSearchViewModel _inventory;
        private bool _scanItemButtonIsVisible;
        private string _itemText;

        #endregion

        #region "Properties"

        public InventoryLookupSearchViewModel Inventory
        {
            get => _inventory;
            set { SetProperty(ref _inventory, value); }
        }

        public bool ScanItemButtonIsVisible
        {
            get => _scanItemButtonIsVisible;
            set { SetProperty(ref _scanItemButtonIsVisible, value); }
        }

        public string ItemText
        {
            get => _itemText;
            set { SetProperty(ref _itemText, value); }
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand ScanCommand
            => new AsyncCommand(Scan,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand SearchCommand
            => new AsyncCommand(Search,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

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

        private async Task Scan()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<InventoryBarcodeScanViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Search()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PushAsync<InventoryLookupViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnOnSelectedInventory(InventoryLookupSearchViewModel e)
        {
            Inventory = e;

            ScanItemButtonIsVisible = false;
        }

        private void OnScanResult(InventoryLookupSearchViewModel e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Inventory = e;

                ScanItemButtonIsVisible = false;
            });
        }

        #endregion

        public InventoryViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            ScanItemButtonIsVisible = true;

            InventoryLookupViewModel.SelectedInventory += OnOnSelectedInventory;
            InventoryBarcodeScanViewModel.ScanResult += OnScanResult;
            BarcodeScanViewModel.InventoryScanResult += OnScanResult;
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}