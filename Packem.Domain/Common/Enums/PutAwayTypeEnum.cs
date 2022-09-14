using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum PutAwayTypeEnum
    {
        [Description("Receive")]
        Receive = 1,
        [Description("Receipt")]
        Receipt = 2,
        [Description("LicensePlate")]
        LicensePlate = 3
    }
}