using System;

namespace Packem.Domain.Models
{
    public class PalletGetModel
    {
        public int PalletId { get; set; }
        public int CustomerLocationId { get; set; }
        public int CustomerFacilityId { get; set; }
        public int LicensePlateId { get; set; }
        public int? BinId { get; set; }
        public int Qty { get; set; } // sum of PalletInventory Qty
        public DateTime CreatedDateTime { get; set; }
    }
}