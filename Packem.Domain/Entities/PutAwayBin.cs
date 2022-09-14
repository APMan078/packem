using Packem.Domain.Common.Interfaces;
using System;

namespace Packem.Domain.Entities
{
    public partial class PutAwayBin : ISoftDelete
    {
        public int PutAwayBinId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? PutAwayId { get; set; }
        public int? BinId { get; set; }
        public int Qty { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual PutAway PutAway { get; set; }
        public virtual Bin Bin { get; set; }
    }
}