using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class CustomerFacility : ISoftDelete
    {
        public CustomerFacility()
        {
            PurchaseOrders = new HashSet<PurchaseOrder>();
            Zones = new HashSet<Zone>();
            SaleOrders = new HashSet<SaleOrder>();
            Recalls = new HashSet<Recall>();
            Pallets = new HashSet<Pallet>();
            PalletInventories = new HashSet<PalletInventory>();
        }

        public int CustomerId { get; set; }
        public int CustomerFacilityId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual ICollection<Zone> Zones { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<SaleOrder> SaleOrders { get; set; }
        public virtual ICollection<Recall> Recalls { get; set; }
        public virtual ICollection<Pallet> Pallets { get; set; }
        public virtual ICollection<PalletInventory> PalletInventories { get; set; }
    }
}