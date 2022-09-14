using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum PickingStatusEnum
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Picking")]
        Picking = 2,
        [Description("Pause")]
        Pause = 3,
        [Description("Complete")]
        Complete = 4
    }
}