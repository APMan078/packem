using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class TransferNew : ISoftDelete
    {
        public TransferNew()
        {
            Transfers = new HashSet<Transfer>();
        }

        public int TransferNewId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? NewZoneId { get; set; }
        public int? NewBinId { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Zone NewZone { get; set; }
        public virtual Bin NewBin { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}