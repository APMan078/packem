using Packem.Domain.Common.Interfaces;

namespace Packem.Domain.Entities
{
    public partial class LicensePlateItem : ISoftDelete
    {
        public int LicensePlateItemId { get; set; }
        public int? CustomerId { get; set; }
        public int? LicensePlateId { get; set; }
        public int? ItemId  { get; set; }
        public int? VendorId { get; set; }
        public int? LotId { get; set; }
        public string ReferenceNo { get; set; }
        public int? Cases { get; set; }
        public int? EaCase { get; set; }
        public int TotalQty { get; set; }
        public int Qty { get; set; }
        public int? TotalWeight { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual LicensePlate LicensePlate { get; set; }
        public virtual Item Item { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual Lot Lot { get; set; }
        public virtual PalletInventory PalletInventory { get; set; }
    }
}