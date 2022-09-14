using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum TransferStatusEnum
    {
        [Description("Pending")]
        Pending = 1,
        [Description("In Progress")]
        InProgress = 2,
        [Description("Completed")]
        Completed = 3,
        [Description("Manual")]
        Manual = 4
    }
}