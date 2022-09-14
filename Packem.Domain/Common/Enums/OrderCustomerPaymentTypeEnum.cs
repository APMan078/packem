using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum OrderCustomerPaymentTypeEnum
    {
        [Description("Sales Order")]
        SalesOrder = 1,
        [Description("Manual")]
        Manual = 2
    }
}