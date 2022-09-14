namespace Packem.Mobile.Models
{
    public class Constants
    {
        public const string MOBILE_STATE = "mobile_state";
        public const string IS_USER_LOGGED_IN = "user_logged";
        //public const string PackemApi = "http://10.0.2.2:5000/api";

        public static class AuthApi
        {
            public const string PostRequestUserToken = "/auth/requestusertoken";
            public const string GetUserTokenInfo = "/auth/usertokeninfo";
            public const string PostValidateCustomerDeviceToken = "/auth/validatecustomerdevicetoken";
            public const string GetCustomerDeviceTokenInfo = "/auth/customerdevicetokeninfo";
            public const string DeactivateCustomerDeviceToken = "/auth/deactivatecustomerdevicetoken";
        }

        public static class CustomerApi
        {
            public const string GetCurrentCustomerForDevice = "/customer/current/device";
        }

        public static class ItemApi
        {
            /// <summary>
            /// GET /api/item/device/lookup/{customerFacilityId}?searchText={searchText}
            /// </summary>
            public const string GetItemLookupDevice = "/item/device/lookup";

            /// <summary>
            /// GET /api/item/device/lookup/sku/{customerFacilityId}?searchText={sku}
            /// </summary>
            public const string GetItemLookupSkuDevice = "/item/device/lookup/sku";
        }

        public static class LotApi
        {
            /// <summary>
            /// GET /api/lot/device/lookup/{itemId}?searchText={searchText}
            /// </summary>
            public const string GetLotLookupDevice = "/lot/device/lookup";

            /// <summary>
            /// // POST /api/lot/device/create
            /// </summary>
            public const string CreateLotDevice = "/lot/device/create";
        }

        public static class ZoneApi
        {
            /// <summary>
            /// GET /api/zone/device/lookup/{customerFacilityId}?searchText={searchText}
            /// </summary>
            public const string GetZoneLookupDevice = "/zone/device/lookup";

            /// <summary>
            /// GET /api/zone/device/lookup/itemquantity/{customerFacilityId}/{itemId}?searchText={searchText}
            /// </summary>
            public const string GetZoneLookupItemQuantityDevice = "/zone/device/lookup/itemquantity";
        }

        public static class BinApi
        {
            /// <summary>
            /// GET /api/bin/device/inventory/{customerFacilityId}/{itemId}
            /// </summary>
            public const string GetBinZoneDevice = "/bin/device/inventory";

            /// <summary>
            /// GET /api/bin/device/lookup/{zoneId}?searchText={searchText}
            /// </summary>
            public const string GetBinLookupDevice = "/bin/device/lookup";

            /// <summary>
            /// GET /api/bin/device/lookup/itemquantitylot/{zoneId}/{itemId}?searchText={searchText}
            /// </summary>
            public const string GetBinItemQuantityLotLookupDevice = "/bin/device/lookup/itemquantitylot";

            /// <summary>
            /// GET /api/bin/device/lookup/itemquantity/{customerFacilityId}/{itemId}/{zoneId}?searchText={searchText}
            /// </summary>
            public const string GetBinLookupItemQuantityDevice = "/bin/device/lookup/itemquantity";

            /// <summary>
            /// GET /api/bin/device/itemquantity/{itemId}/{binId}
            /// </summary>
            public const string GetBinItemQuantityDevice = "/bin/device/itemquantity";

            /// <summary>
            /// GET /api/bin/device/lookup/pallet/optional/{zoneId}?searchText={searchText}
            /// </summary>
            public const string GetBinLookupOptionalPalletDevice = "/bin/device/lookup/pallet/optional";
        }

        public static class PurchaseOrderApi
        {
            /// <summary>
            /// GET /api/purchaseorder/device/lookup/poreceive/{customerFacilityId}?type={type}&searchText={searchText}
            /// </summary>
            public const string GetPurchaseOrderLookupDevice = "/purchaseorder/device/lookup/poreceive";

            /// <summary>
            /// PUT /api/purchaseorder/device/updatestatustoputaway/{purchaseOrderId}
            /// </summary>
            public const string UpdatePurchaseOrderStatusToPutAwayDevice = "/purchaseorder/device/updatestatustoputaway";
        }

        public static class ReceiveApi
        {
            /// <summary>
            /// GET /api/receive/device/lookup/poreceive/{customerFacilityId}/{purchaseOrderId}?searchText={searchText}
            /// </summary>
            public const string GetPurchaseOrderLookupPOReceiveDevice = "/receive/device/lookup/poreceive";

            /// <summary>
            /// GET /api/receive/device/updatestatustoreceived/{receiveId}
            /// </summary>
            public const string UpdateReceiveStatusToReceivedDevice = "/receive/device/updatestatustoreceived";
        }

        public static class PutAwayApi
        {
            /// <summary>
            /// POST /api/putaway/device/createputaway
            /// </summary>
            public const string CreatePutAwayDevice = "/putaway/device/createputaway";

            /// <summary>
            /// POST /api/putaway/device/bin/createputawaybin
            /// </summary>
            public const string CreatePutAwayBinDevice = "/putaway/device/bin/createputawaybin";

            /// <summary>
            /// GET /api/putaway/device/lookup/{customerFacilityId}?searchText={searchText}&skuSearch={skuSearch}
            /// </summary>
            public const string GetPutAwayLookupDevice = "/putaway/device/lookup";

            /// <summary>
            /// GET /api/putaway/device/{putAwayId}
            /// </summary>
            public const string GetPutAwayDevice = "/putaway/device";

            /// <summary>
            /// GET /api/putaway/device/lookup/licenseplate/{customerFacilityId}?searchText={searchText}&barcodeSearch={barcodeSearch}
            /// </summary>
            public const string GetPutAwayLookupLicensePlateDevice = "/putaway/device/lookup/licenseplate";

            /// <summary>
            /// POST /api/putaway/device/bin/createputawaybin/pallet
            /// </summary>
            public const string CreatePutAwayPalletDevice = "/putaway/device/bin/createputawaybin/pallet";
        }

        public static class ReceiptApi
        {
            /// <summary>
            /// POST /api/receipt/device/createreceipt
            /// </summary>
            public const string CreateReceiptDevice = "/receipt/device/createreceipt";
        }

        public static class TransferApi
        {
            /// <summary>
            /// GET /api/transfer/device/lookup/{customerFacilityId}?searchText={searchText}
            /// </summary>
            public const string GetTransferLookupDevice = "/transfer/device/lookup";

            /// <summary>
            /// POST /api/transfer/device/manualtransfer
            /// </summary>
            public const string CreateTransferManualDevice = "/transfer/device/manualtransfer";

            /// <summary>
            /// POST /api/transfer/device/transferrequest
            /// </summary>
            public const string CreateTransferRequestDevice = "/transfer/device/transferrequest";
        }

        public static class SaleOrderApi
        {
            /// <summary>
            /// GET /api/saleorder/device/lookup/{customerFacilityId}?searchText={searchText}&skuSearch={barcodeSearch}
            /// </summary>
            public const string GetSaleOrderPickQueueLookupDevice = "/saleorder/device/lookup";

            /// <summary>
            /// POST /api/saleorder/device/update/pickingstatus
            /// </summary>
            public const string UpdateSaleOrderPickingStatusDevice = "/saleorder/device/update/pickingstatus";
        }

        public static class OrderLineApi
        {
            /// <summary>
            /// GET /api/orderline/device/{orderLineId}
            /// </summary>
            public const string GetOrderLineDevice = "/orderline/device";

            /// <summary>
            /// GET /api/orderline/device/lookup/{saleOrderId}?searchText={searchText}&skuSearch={barcodeSearch}
            /// </summary>
            public const string GetOrderLinePickLookupDevice = "/orderline/device/lookup";

            /// <summary>
            /// POST /api/orderline/device/bin/createorderlinebin
            /// </summary>
            public const string CreateOrderLineBinDevice = "/orderline/device/bin/createorderlinebin";
        }

        public static class RecallApi
        {
            /// <summary>
            /// POST /api/recall/device/update/status
            /// </summary>
            public const string UpdateRecallStatusDevice = "/recall/device/update/status";

            /// <summary>
            /// GET /api/recall/device/lookup/{customerFacilityId}?searchText={searchText}&barcodeSearch={barcodeSearch}
            /// </summary>
            public const string GetRecallQueueLookupDevice = "/recall/device/lookup";

            /// <summary>
            /// GET /api/recall/device/detail/{recallId}
            /// </summary>
            public const string GetRecallDetailDevice = "/recall/device/detail";

            /// <summary>
            /// GET /api/recall/device/detail/{recallId}/{binId}
            /// </summary>
            public const string GetRecallDetailDevice2 = "/recall/device/detail";

            /// <summary>
            /// POST /api/recall/device/bin/createrecallbin
            /// </summary>
            public const string CreateRecallBinDevice = "/recall/device/bin/createrecallbin";
        }

        public static class LicensePlateApi
        {
            /// <summary>
            /// GET /api/licenseplate/device/lookup?searchText={searchText}&barcodeSearch={barcodeSearch}
            /// </summary>
            public const string GetLicensePlateLookupDevice = "/licenseplate/device/lookup";

            /// <summary>
            /// GET /api/licenseplate/device/assignment/{licensePlateId}
            /// </summary>
            public const string GetLicensePlateKnownAssignmentDevice = "/licenseplate/device/assignment";

            /// <summary>
            /// POST /api/licenseplate/device/edit/unknown
            /// </summary>
            public const string EditLicensePlateUnknownToPalletizedDevice = "/licenseplate/device/edit/unknown";

            /// <summary>
            /// POST /api/licenseplate/device/edit/known
            /// </summary>
            public const string EditLicensePlateKnownToPalletizedDevice = "/licenseplate/device/edit/known";
        }

        public static class PalletApi
        {
        }

        public static class VendorApi
        {
            /// <summary>
            /// GET /api/vendor/device/lookup/name/{itemId}?searchText={searchText}
            /// </summary>
            public const string GetVendorLookupByNameDevice = "/vendor/device/lookup/name";
        }
    }
}