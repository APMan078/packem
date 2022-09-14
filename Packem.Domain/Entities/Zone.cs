using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Zone : ISoftDelete
    {
        public Zone()
        {
            InventoryZones = new HashSet<InventoryZone>();
            Bins = new HashSet<Bin>();
            ActivityLogs = new HashSet<ActivityLog>();
            TransferCurrents = new HashSet<TransferCurrent>();
            TransferNews = new HashSet<TransferNew>();
            TransferZoneBins = new HashSet<TransferZoneBin>();
        }

        public int ZoneId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual CustomerFacility CustomerFacility { get; set; }
        public virtual ICollection<InventoryZone> InventoryZones { get; set; }
        public virtual ICollection<Bin> Bins { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
        public virtual ICollection<TransferCurrent> TransferCurrents { get; set; }
        public virtual ICollection<TransferNew> TransferNews { get; set; }
        public virtual ICollection<TransferZoneBin> TransferZoneBins { get; set; }
    }
}