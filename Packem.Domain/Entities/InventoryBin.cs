using Packem.Domain.Common.Interfaces;

namespace Packem.Domain.Entities
{
    public partial class InventoryBin : ISoftDelete
    {
        public int InventoryBinId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? InventoryId { get; set; }
        public int? BinId { get; set; }
        public int? LotId { get; set; }
        public int Qty { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual Bin Bin { get; set; }
        public virtual Lot Lot { get; set; }
    }
}