using System;

namespace Packem.Domain.Models
{
    public class PurchaseOrderImportModel
    {
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string ShipVia { get; set; }
        public int OrderQty { get; set; }
        public DateTime? OrderDate { get; set; }
        public string VendorName { get; set; }
        public string VendorAccount { get; set; }
        public string PointOfCOntact { get; set; }
        public string VendorPhone { get; set; }
        public string VendorAddress { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCity { get; set; }
        public string VendorZip { get; set; }
        public string VendorStateOrProvince { get; set; }
        public string ItemSKU { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUoM { get; set; }
        public int ItemOrderQty { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }



    }
}

