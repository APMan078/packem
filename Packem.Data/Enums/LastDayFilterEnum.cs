using System.ComponentModel;

namespace Packem.Data.Enums
{
    public enum LastDayFilterEnum
    {
        [Description("YTD")]
        YTD = 1, // last 365 days
        [Description("MTD")]
        MTD = 2, // last 30 days
        [Description("WTD")]
        WTD = 3 // last 7 days
    }
}