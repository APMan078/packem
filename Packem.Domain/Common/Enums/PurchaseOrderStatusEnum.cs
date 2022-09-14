using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum PurchaseOrderStatusEnum
    {
        [Description("Not Received")]
        NotReceived = 1,
        [Description("Receiving")]
        Receiving = 2,
        [Description("Closed")]
        Closed = 3
    }
}
