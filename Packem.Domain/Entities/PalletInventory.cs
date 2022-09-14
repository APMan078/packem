using Packem.Domain.Common.Interfaces;

namespace Packem.Domain.Entities
{
    public partial class PalletInventory : ISoftDelete
    {
        public int PalletInventoryId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public int? PalletId { get; set; }
        public int? InventoryId { get; set; }
        public int? LicensePlateItemId { get; set; }
        public int Qty { get; set; } // each qty
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual CustomerFacility CustomerFacility { get; set; }
        public virtual Pallet Pallet { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual LicensePlateItem LicensePlateItem { get; set; }
    }
}