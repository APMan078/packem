using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class LicensePlateKnownAssignmentDeviceGetModel
    {
        public class Product
        {
            public int LicensePlateItemId { get; set; }
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string ItemDescription { get; set; }
            public int? VendorId { get; set; }
            public string VendorName { get; set; }
            public int? LotId { get; set; }
            public string LotNo { get; set; }
            public string ReferenceNo { get; set; }
            public int? Cases { get; set; }
            public int? EaCase { get; set; }
            public int TotalQty { get; set; }
            public int? TotalWeight { get; set; }
        }

        public LicensePlateKnownAssignmentDeviceGetModel()
        {
            Products = new List<Product>();
        }

        public int? LicensePlateId { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
    }
}