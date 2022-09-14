using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum SaleOrderStatusEnum
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Printed")]
        Printed = 2
    }
}