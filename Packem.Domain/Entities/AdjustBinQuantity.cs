using Packem.Domain.Common.Interfaces;
using System;

namespace Packem.Domain.Entities
{
    public partial class AdjustBinQuantity : ISoftDelete
    {
        public int AdjustBinQuantityId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ItemId { get; set; }
        public int? BinId { get; set; }
        public int OldQty { get; set; }
        public int NewQty { get; set; }
        public DateTime AdjustDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Item Item { get; set; }
        public virtual Bin Bin { get; set; }
    }
}