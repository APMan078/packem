using System;
using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum BinCategoryEnum
    {
        [Description("Unassigned")]
        Unassigned = 1,
        [Description("Picking")]
        Picking = 2,
        [Description("Bulk")]
        Bulk = 3,
    }
}

