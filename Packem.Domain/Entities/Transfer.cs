using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Transfer : ISoftDelete
    {
        public Transfer()
        {
            TransferZoneBins = new HashSet<TransferZoneBin>();
        }

        public int TransferId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ItemId { get; set; }
        public int? TransferCurrentId { get; set; }
        public int? TransferNewId { get; set; }
        public int Qty { get; set; }
        public int Remaining { get; set; }
        public TransferStatusEnum Status { get; set; }
        public DateTime TransferDateTime { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Item Item { get; set; }
        public virtual TransferCurrent TransferCurrent { get; set; }
        public virtual TransferNew TransferNew { get; set; }
        public virtual ICollection<TransferZoneBin> TransferZoneBins { get; set; }
    }
}