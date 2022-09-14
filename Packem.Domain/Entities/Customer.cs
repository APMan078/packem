using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Customer : ISoftDelete
    {
        public Customer()
        {
            CustomerLocations = new HashSet<CustomerLocation>();
            Users = new HashSet<User>();
            CustomerDevices = new HashSet<CustomerDevice>();
            Items = new HashSet<Item>();
            Vendors = new HashSet<Vendor>();
            ItemVendors = new HashSet<ItemVendor>();
            Inventories = new HashSet<Inventory>();
            ActivityLogs = new HashSet<ActivityLog>();
            UnitOfMeasures = new HashSet<UnitOfMeasure>();
            UnitOfMeasureCustomers = new HashSet<UnitOfMeasureCustomer>();
            OrderCustomers = new HashSet<OrderCustomer>();
            LicensePlates = new HashSet<LicensePlate>();
            LicensePlateItems = new HashSet<LicensePlateItem>();
        }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PointOfContact { get; set; }
        public string ContactEmail { get; set; }
        public bool IsActive { get; set; }
        public int? DefaultItemThreshold { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<CustomerLocation> CustomerLocations { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<CustomerDevice> CustomerDevices { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Vendor> Vendors { get; set; }
        public virtual ICollection<ItemVendor> ItemVendors { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
        public virtual ICollection<UnitOfMeasure> UnitOfMeasures { get; set; }
        public virtual ICollection<UnitOfMeasureCustomer> UnitOfMeasureCustomers { get; set; }
        public virtual ICollection<OrderCustomer> OrderCustomers { get; set; }
        public virtual ICollection<LicensePlate> LicensePlates { get; set; }
        public virtual ICollection<LicensePlateItem> LicensePlateItems { get; set; }
    }
}