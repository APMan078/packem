using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class ItemDetailGetModel
    {
        public class CustomerLocation
        {
            public int CustomerLocationId { get; set; }
            public int CustomerFacilityId { get; set; }
            public string Facility { get; set; }
            public int ZoneId { get; set; }
            public string Zone { get; set; }
            public int BinId { get; set; }
            public string Bin { get; set; }
            public int QtyOnHand { get; set; }
            public string LotNo { get; set; }
            public string ExpirationDate { get; set; }
        }

        public class Vendor
        {
            public int VendorId { get; set; }
            public string VendorNo { get; set; }
            public string Name { get; set; }
            public string Contact { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
        }

        public class Activity
        {
            public int ActivityLogId { get; set; }
            public string Type { get; set; }
            public string User { get; set; }
            public string Date { get; set; }
            public string Qty { get; set; }
            public string Zone { get; set; }
            public string BinLocation { get; set; }
        }

        public ItemDetailGetModel()
        {
            CustomerLocations = new List<CustomerLocation>();
            Vendors = new List<Vendor>();
            Activities = new List<Activity>();
        }

        public IEnumerable<CustomerLocation> CustomerLocations { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
    }
}