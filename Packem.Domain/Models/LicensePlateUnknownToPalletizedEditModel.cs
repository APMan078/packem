using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class LicensePlateUnknownToPalletizedEditModel
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

        public LicensePlateUnknownToPalletizedEditModel()
        {
            Products = new List<Product>();
        }

        public int? LicensePlateId { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}