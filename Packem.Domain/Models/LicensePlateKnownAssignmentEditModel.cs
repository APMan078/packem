using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class LicensePlateKnownAssignmentEditModel
    {
        public class Product
        {
            public int? LicensePlateItemId { get; set; }
            public int? ItemId { get; set; }
            public int? VendorId { get; set; }
            public int? LotId { get; set; }
            public string ReferenceNo { get; set; }
            public int? Cases { get; set; }
            public int? EaCase { get; set; }
            public int? TotalQty { get; set; }
            public int? TotalWeight { get; set; }
        }

        public LicensePlateKnownAssignmentEditModel()
        {
            Products = new List<Product>();
        }

        public int? CustomerId { get; set; }
        public int? LicensePlateId { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
    }
}