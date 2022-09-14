using Packem.Domain.Common.Interfaces;
using System;

namespace Packem.Domain.Entities
{
    public partial class TransferZoneBin : ISoftDelete
    {
        public int TransferZoneBinId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? TransferId { get; set; }
        public int? ZoneId { get; set; }
        public int? BinId { get; set; }
        public int Qty { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Transfer Transfer { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual Bin Bin { get; set; }
    }
}