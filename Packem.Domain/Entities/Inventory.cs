using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Inventory : ISoftDelete
    {
        public Inventory()
        {
            InventoryZones = new HashSet<InventoryZone>();
            InventoryBins = new HashSet<InventoryBin>();
            ActivityLogs = new HashSet<ActivityLog>();
            PalletInventories = new HashSet<PalletInventory>();
        }

        public int InventoryId { get; set; }
        public int? CustomerId { get; set; }
        public int? ItemId { get; set; }
        public int QtyOnHand { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<InventoryZone> InventoryZones { get; set; }
        public virtual ICollection<InventoryBin> InventoryBins { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
        public virtual ICollection<PalletInventory> PalletInventories { get; set; }
    }
}