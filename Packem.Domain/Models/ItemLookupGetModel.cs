using System;

namespace Packem.Domain.Models
{
    public class ItemLookupGetModel
    {
        public int ItemId { get; set; }
        public string ItemSKU { get; set; }
        public string UOM { get; set; }
        public int BinLocations { get; set; }
        public int QtyOnHand { get; set; }
        public string Description { get; set; }
        public int Vendors { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Threshold { get; set; }
    }
}