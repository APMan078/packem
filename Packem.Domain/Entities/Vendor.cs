using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Vendor : ISoftDelete
    {
        public Vendor()
        {
            ItemVendors = new HashSet<ItemVendor>();
            PurchaseOrders = new HashSet<PurchaseOrder>();
            UserRoleVendors = new HashSet<UserRoleVendor>();
            LicensePlateItems = new HashSet<LicensePlateItem>();
        }

        public int VendorId { get; set; }
        public int? CustomerId { get; set; }
        public string VendorNo { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string PointOfContact { get; set; }
        public string PhoneNumber { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<ItemVendor> ItemVendors { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<UserRoleVendor> UserRoleVendors { get; set; }
        public virtual ICollection<LicensePlateItem> LicensePlateItems { get; set; }
    }
}