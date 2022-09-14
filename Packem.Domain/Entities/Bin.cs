using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;
using Packem.Domain.Common.Enums;

namespace Packem.Domain.Entities
{
    public partial class Bin : ISoftDelete
    {
        public Bin()
        {
            InventoryBins = new HashSet<InventoryBin>();
            ActivityLogs = new HashSet<ActivityLog>();
            PutAwayBins = new HashSet<PutAwayBin>();
            TransferCurrents = new HashSet<TransferCurrent>();
            TransferNews = new HashSet<TransferNew>();
            AdjustBinQuantities = new HashSet<AdjustBinQuantity>();
            TransferZoneBins = new HashSet<TransferZoneBin>();
            OrderLineBins = new HashSet<OrderLineBin>();
            RecallBins = new HashSet<RecallBin>();
            Pallets = new HashSet<Pallet>();
        }

        public int BinId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ZoneId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public BinCategoryEnum Category { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual ICollection<InventoryBin> InventoryBins { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
        public virtual ICollection<PutAwayBin> PutAwayBins { get; set; }
        public virtual ICollection<TransferCurrent> TransferCurrents { get; set; }
        public virtual ICollection<TransferNew> TransferNews { get; set; }
        public virtual ICollection<AdjustBinQuantity> AdjustBinQuantities { get; set; }
        public virtual ICollection<TransferZoneBin> TransferZoneBins { get; set; }
        public virtual ICollection<OrderLineBin> OrderLineBins { get; set; }
        public virtual ICollection<RecallBin> RecallBins { get; set; }
        public virtual ICollection<Pallet> Pallets { get; set; }
    }
}