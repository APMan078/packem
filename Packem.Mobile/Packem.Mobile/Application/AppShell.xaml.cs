using Autofac;
using Packem.Mobile.Application;
using Packem.Mobile.Modules.Barcode;
using Packem.Mobile.Modules.Inventories;
using Packem.Mobile.Modules.Palletize;
using Packem.Mobile.Modules.Picking;
using Packem.Mobile.Modules.PurchaseOrders;
using Packem.Mobile.Modules.PutAways;
using Packem.Mobile.Modules.Recalls;
using Packem.Mobile.Modules.Receipts;
using Packem.Mobile.Modules.Transfers;
using Xamarin.Forms;

namespace Packem.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("MainMenuViewModel/InventoryViewModel", typeof(InventoryView));
            Routing.RegisterRoute("MainMenuViewModel/InventoryViewModel/InventoryLookupViewModel", typeof(InventoryLookupView));
            Routing.RegisterRoute("MainMenuViewModel/InventoryViewModel/InventoryBarcodeScanViewModel", typeof(InventoryBarcodeScanView));

            Routing.RegisterRoute("MainMenuViewModel/PurchaseOrderReceiveViewModel", typeof(PurchaseOrderReceiveView));
            Routing.RegisterRoute("MainMenuViewModel/PurchaseOrderReceiveViewModel/PurchaseOrderReceivePrintViewModel", typeof(PurchaseOrderReceivePrintView));
            Routing.RegisterRoute("MainMenuViewModel/PurchaseOrderReceiveViewModel/PurchaseOrderReceivePrintWebViewModel", typeof(PurchaseOrderReceivePrintWebView));
            Routing.RegisterRoute("MainMenuViewModel/PurchaseOrderReceiveViewModel/OrderLookupViewModel", typeof(OrderLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PurchaseOrderReceiveViewModel/OrderScanViewModel", typeof(OrderScanView));
            Routing.RegisterRoute("MainMenuViewModel/PurchaseOrderReceiveViewModel/ItemLookupViewModel", typeof(ItemLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PurchaseOrderReceiveViewModel/ItemScanViewModel", typeof(ItemScanView));

            Routing.RegisterRoute("MainMenuViewModel/ReceiptViewModel", typeof(ReceiptView));
            Routing.RegisterRoute("MainMenuViewModel/ReceiptViewModel/ReceiptItemScanViewModel", typeof(ReceiptItemScanView));
            Routing.RegisterRoute("MainMenuViewModel/ReceiptViewModel/ReceiptItemLookupViewModel", typeof(ReceiptItemLookupView));
            Routing.RegisterRoute("MainMenuViewModel/ReceiptViewModel/ReceiptAddLotViewModel", typeof(ReceiptAddLotView));
            Routing.RegisterRoute("MainMenuViewModel/ReceiptViewModel/ReceiptLotLookupViewModel", typeof(ReceiptLotLookupView));
            Routing.RegisterRoute("MainMenuViewModel/ReceiptViewModel/ReceiptPrintWebViewModel", typeof(ReceiptPrintWebView));

            Routing.RegisterRoute("MainMenuViewModel/PalletizeViewModel", typeof(PalletizeView));
            Routing.RegisterRoute("MainMenuViewModel/PalletizeViewModel/PalletizeLicensePlateLookupViewModel", typeof(PalletizeLicensePlateLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PalletizeViewModel/PalletizeLicensePlateLookupViewModel/PalletizeLicensePlateScanViewModel", typeof(PalletizeLicensePlateScanView));
            Routing.RegisterRoute("MainMenuViewModel/PalletizeViewModel/PalletizeItemScanViewModel", typeof(PalletizeItemScanView));
            Routing.RegisterRoute("MainMenuViewModel/PalletizeViewModel/PalletizeItemLookupViewModel", typeof(PalletizeItemLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PalletizeViewModel/PalletizeAddLotViewModel", typeof(PalletizeAddLotView));

            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel", typeof(PutAwayView));
            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel/PutAwayItemLookupViewModel", typeof(PutAwayItemLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel/PutAwayZoneLookupViewModel", typeof(PutAwayZoneLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel/PutAwayBinLookupViewModel", typeof(PutAwayBinLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel/PutAwayBinScanViewModel", typeof(PutAwayBinScanView));
            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel/PutAwayLicensePlateScanViewModel", typeof(PutAwayLicensePlateScanView));
            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel/PutAwayLicensePlateLookupViewModel", typeof(PutAwayLicensePlateLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PutAwayViewModel/PutAwayBinLookupPalletViewModel", typeof(PutAwayBinLookupPalletView));

            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel", typeof(PickQueueView));
            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel/PickQueueScanViewModel", typeof(PickQueueScanView));
            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel/PickQueueItemLookupViewModel", typeof(PickQueueItemLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel/PickQueueItemLookupViewModel/PickQueueItemScanViewModel", typeof(PickQueueItemScanView));
            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel/PickQueueItemLookupViewModel/PickingViewModel", typeof(PickingView));
            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel/PickQueueItemLookupViewModel/PickingViewModel/PickingZoneLookupViewModel", typeof(PickingZoneLookupView));
            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel/PickQueueItemLookupViewModel/PickingViewModel/PickingBinScanViewModel", typeof(PickingBinScanView));
            Routing.RegisterRoute("MainMenuViewModel/PickQueueViewModel/PickQueueItemLookupViewModel/PickingViewModel/PickingBinLookupViewModel", typeof(PickingBinLookupView));

            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel", typeof(TransferView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferItemScanViewModel", typeof(TransferItemScanView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferRequestViewModel", typeof(TransferRequestView));

            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferRequestViewModel/TransferRequestZoneToLookupViewModel", typeof(TransferRequestZoneToLookupView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferRequestViewModel/TransferRequestBinToScanViewModel", typeof(TransferRequestBinToScanView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferRequestViewModel/TransferRequestBinToLookupViewModel", typeof(TransferRequestBinToLookupView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel", typeof(TransferManualView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemScanViewModel", typeof(TransferManualItemScanView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemLookupViewModel", typeof(TransferManualItemLookupView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemZoneFromLookupViewModel", typeof(TransferManualItemZoneFromLookupView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemBinFromScanViewModel", typeof(TransferManualItemBinFromScanView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemBinFromLookupViewModel", typeof(TransferManualItemBinFromLookupView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemZoneToLookupViewModel", typeof(TransferManualItemZoneToLookupView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemBinToScanViewModel", typeof(TransferManualItemBinToScanView));
            Routing.RegisterRoute("MainMenuViewModel/TransferViewModel/TransferManualViewModel/TransferManualItemBinToLookupViewModel", typeof(TransferManualItemBinToLookupView));

            Routing.RegisterRoute("MainMenuViewModel/RecallQueueViewModel", typeof(RecallQueueView));
            Routing.RegisterRoute("MainMenuViewModel/RecallQueueViewModel/RecallQueueScanViewModel", typeof(RecallQueueScanView));
            Routing.RegisterRoute("MainMenuViewModel/RecallQueueViewModel/RecallItemViewModel", typeof(RecallItemView));
            Routing.RegisterRoute("MainMenuViewModel/RecallQueueViewModel/RecallDetailViewModel/RecallPickingViewModel", typeof(RecallPickingView));
            Routing.RegisterRoute("MainMenuViewModel/RecallQueueViewModel/RecallDetailViewModel/RecallPickingViewModel/RecallPickingBinScanViewModel", typeof(RecallPickingBinScanView));
            Routing.RegisterRoute("MainMenuViewModel/RecallQueueViewModel/RecallDetailViewModel/RecallPickingViewModel/RecallPickingBinLookupViewModel", typeof(RecallPickingBinLookupView));

            Routing.RegisterRoute("MainMenuViewModel/BarcodeScanViewModel", typeof(BarcodeScanView));

            BindingContext = App.Container.Resolve<AppShellViewModel>();
        }
    }
}