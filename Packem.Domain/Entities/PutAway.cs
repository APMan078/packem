using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class PutAway : ISoftDelete
    {
        public PutAway()
        {
            PutAwayBins = new HashSet<PutAwayBin>();
        }

        public int PutAwayId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ReceiveId { get; set; }
        public int? ReceiptId { get; set; }
        public int? LicensePlateId { get; set; }
        public int Qty { get; set; }
        public int Remaining { get; set; }
        public PutAwayTypeEnum PutAwayType { get; set; }
        public DateTime PutAwayDate { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Receive Receive { get; set; }
        public virtual Receipt Receipt { get; set; }
        public virtual LicensePlate LicensePlate { get; set; }
        public virtual ICollection<PutAwayBin> PutAwayBins { get; set; }
    }
}