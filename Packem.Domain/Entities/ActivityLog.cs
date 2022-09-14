using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;

namespace Packem.Domain.Entities
{
    public class ActivityLog : ISoftDelete
    {
        public int ActivityLogId { get; set; }
        public int? CustomerId { get; set; }
        public ActivityLogTypeEnum Type { get; set; }
        public int? InventoryId { get; set; }
        public int? UserId { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public int Qty { get; set; }
        public int OldQty { get; set; }
        public int NewQty { get; set; }
        public MathematicalSymbolEnum? MathematicalSymbol { get; set; }
        public int? ZoneId { get; set; }
        public int? BinId { get; set; }
        //public string Notes { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual User User { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual Bin Bin { get; set; }
    }
}