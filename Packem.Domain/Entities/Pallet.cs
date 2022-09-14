using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Pallet : ISoftDelete
    {
        public Pallet()
        {
            LicensePlates = new HashSet<LicensePlate>();
            PalletInventories = new HashSet<PalletInventory>();
        }

        public int PalletId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public int? BinId { get; set; }
        public int Qty { get; set; } // sum of PalletInventory Qty
        public DateTime CreatedDateTime { get; set; }
        public bool MixedPallet { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual CustomerFacility CustomerFacility { get; set; }
        public virtual ICollection<LicensePlate> LicensePlates { get; set; }
        public virtual Bin Bin { get; set; }
        public virtual ICollection<PalletInventory> PalletInventories { get; set; }
    }
}