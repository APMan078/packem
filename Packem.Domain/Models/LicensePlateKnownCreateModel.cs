using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class LicensePlateKnownCreateModel
    {
        public class Product
        {
            public int? ItemId { get; set; }
            public int? VendorId { get; set; }
            public int? LotId { get; set; }
            public string ReferenceNo { get; set; }
            public int? Cases { get; set; }
            public int? EaCase { get; set; }
            public int? TotalQty { get; set; }
            public int? TotalWeight { get; set; }
        }

        public LicensePlateKnownCreateModel()
        {
            Products = new List<Product>();
        }

        public int? CustomerId { get; set; }
        public string LicensePlateNo { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
    }
}