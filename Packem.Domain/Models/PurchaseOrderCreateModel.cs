using System;

namespace Packem.Domain.Models
{
    public class PurchaseOrderCreateModel
    {
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string ShipVia { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
