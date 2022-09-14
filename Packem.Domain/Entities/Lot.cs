using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Lot : ISoftDelete
    {
        public Lot()
        {
            InventoryBins = new HashSet<InventoryBin>();
            Receives = new HashSet<Receive>();
            Receipts = new HashSet<Receipt>();
            LicensePlateItems = new HashSet<LicensePlateItem>();
        }

        public int LotId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ItemId { get; set; }
        public string LotNo { get; set; } // unique at item level
        public DateTime ExpirationDate { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<InventoryBin> InventoryBins { get; set; }
        public virtual ICollection<Receive> Receives { get; set; }
        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual ICollection<LicensePlateItem> LicensePlateItems { get; set; }
    }
}