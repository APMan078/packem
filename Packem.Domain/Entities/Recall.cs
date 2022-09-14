using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Recall : ISoftDelete
    {
        public Recall()
        {
            RecallBins = new HashSet<RecallBin>();
        }

        public int RecallId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public int? ItemId { get; set; }
        public DateTime RecallDate { get; set; }
        public RecallStatusEnum Status { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual CustomerFacility CustomerFacility { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<RecallBin> RecallBins { get; set; }
    }
}