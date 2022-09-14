using Packem.Domain.Common.Interfaces;

namespace Packem.Domain.Entities
{
    public partial class InventoryZone : ISoftDelete
    {
        public int InventoryZoneId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? InventoryId { get; set; }
        public int? ZoneId { get; set; }
        public int Qty { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual Zone Zone { get; set; }
    }
}
