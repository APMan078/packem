using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Modules.Barcode;
using Packem.Mobile.Modules.Inventories;
using Packem.Mobile.Modules.Palletize;
using Packem.Mobile.Modules.Picking;
using Packem.Mobile.Modules.PurchaseOrders;
using Packem.Mobile.Modules.PutAways;
using Packem.Mobile.Modules.Recalls;
using Packem.Mobile.Modules.Receipts;
using Packem.Mobile.Modules.Transfers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Packem.Mobile.Modules.Menus
{
    public class MainMenuViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;

        #endregion

        #region "Properties"
        #endregion

        #region "Commands"

        public ICommand InventoryCommand
            => new AsyncCommand(Inventory,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        public ICommand POReceiveCommand
            => new AsyncCommand(POReceive,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        public ICommand ReceiptCommand
            => new AsyncCommand(Receipt,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        public ICommand PalletizeCommand
            => new AsyncCommand(Palletize,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        public ICommand PutAwayCommand
            => new AsyncCommand(PutAway,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        public ICommand PickingCommand
            => new AsyncCommand(Picking,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        public ICommand TransferCommand
            => new AsyncCommand(Transfer,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        public ICommand RecallCommand
           => new AsyncCommand(Recall,
               canExecute: x => IsNotBusy,
               onException: x => Console.WriteLine(x),
               allowsMultipleExecutions: false);

        public ICommand ScanCommand
            => new AsyncCommand(Scan,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x),
                allowsMultipleExecutions: false);

        #endregion

        #region "Functions"

        private async Task<bool> HasCameraPermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();

                    if (status == PermissionStatus.Granted)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task Scan()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<BarcodeScanViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Inventory()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<InventoryViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task POReceive()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<PurchaseOrderReceiveViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Receipt()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<ReceiptViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Palletize()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<PalletizeViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PutAway()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<PutAwayViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Picking()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<PickQueueViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Transfer()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<TransferViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Recall()
        {
            try
            {
                IsBusy = true;

                if (await HasCameraPermission())
                {
                    await _navigationService.PushAsync<RecallQueueViewModel>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public MainMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}