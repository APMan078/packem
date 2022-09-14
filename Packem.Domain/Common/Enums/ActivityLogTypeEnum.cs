using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum ActivityLogTypeEnum
    {
        [Description("AddBin")] // web, Inventory > Item > Add Bin & Qty
        AddBin = 1,
        [Description("Adjustment")] // web, Inventory > Item > Locations > Adjust
        Adjustment = 2,
        [Description("Purchase Order")] // mobile
        PurchaseOrder = 3,
        [Description("Receipt")] // mobile
        Receipt = 4,
        [Description("Sale Order")] // mobile
        SaleOrder = 5,
        [Description("Transfer")] // mobile
        Transfer = 6,
        [Description("Transfer (Manual)")] // mobile
        TransferManual = 7,
        [Description("Recall")] // mobile
        Recall = 8,
        [Description("Palletize")] // mobile
        Palletize = 9
    }
}