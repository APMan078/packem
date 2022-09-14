using System.ComponentModel;

namespace Packem.Integrations.Enums
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
