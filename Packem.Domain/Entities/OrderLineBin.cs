using Packem.Domain.Common.Interfaces;
using System;

namespace Packem.Domain.Entities
{
    public partial class OrderLineBin : ISoftDelete
    {
        public int OrderLineBinId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? OrderLineId { get; set; }
        public int? BinId { get; set; }
        public int Qty { get; set; }
        public DateTime PickDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual OrderLine OrderLine { get; set; }
        public virtual Bin Bin { get; set; }
    }
}