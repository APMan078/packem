using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class TransferCurrent : ISoftDelete
    {
        public TransferCurrent()
        {
            Transfers = new HashSet<Transfer>();
        }

        public int TransferCurrentId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CurrentZoneId { get; set; }
        public int? CurrentBinId { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Zone CurrentZone { get; set; }
        public virtual Bin CurrentBin { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}