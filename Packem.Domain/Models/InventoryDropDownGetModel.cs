using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class InventoryDropDownGetModel
    {
        public class UnitOfMeasure
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public class CustomerLocation
        {
            public int CustomerLocationId { get; set; }
            public string Name { get; set; }
        }

        public class CustomerFacility
        {
            public int CustomerFacilityId { get; set; }
            public int CustomerLocationId { get; set; }
            public string Name { get; set; }
        }

        public class Zone
        {
            public int ZoneId { get; set; }
            public int CustomerLocationId { get; set; }
            public int CustomerFacilityId { get; set; }
            public string Name { get; set; }
        }

        public class Bin
        {
            public int? BinId { get; set; }
            public int? CustomerLocationId { get; set; }
            public int? ZoneId { get; set; }
            public string Name { get; set; }
        }

        public class Vendor
        {
            public int? VendorId { get; set; }
            public int? CustomerId { get; set; }
            public string VendorNo { get; set; }
            public string Name { get; set; }
            public string PointOfContact { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
        }

        public InventoryDropDownGetModel()
        {
            UnitOfMeasures = new List<UnitOfMeasure>();
            CustomerLocations = new List<CustomerLocation>();
            CustomerFacilities = new List<CustomerFacility>();
            Zones = new List<Zone>();
            Bins = new List<Bin>();
            Vendors = new List<Vendor>();
        }

        public IEnumerable<UnitOfMeasure> UnitOfMeasures { get; set; }
        public IEnumerable<CustomerLocation> CustomerLocations { get; set; }
        public IEnumerable<CustomerFacility> CustomerFacilities { get; set; }
        public IEnumerable<Zone> Zones { get; set; }
        public IEnumerable<Bin> Bins { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }
    }
}
