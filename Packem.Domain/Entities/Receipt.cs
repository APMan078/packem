using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Receipt : ISoftDelete
    {
        public Receipt()
        {
            PutAways = new HashSet<PutAway>();
        }

        public int ReceiptId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ItemId { get; set; }
        public int Qty { get; set; }
        public int Remaining { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int? LotId { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<PutAway> PutAways { get; set; }
        public virtual Lot Lot { get; set; }
    }
}