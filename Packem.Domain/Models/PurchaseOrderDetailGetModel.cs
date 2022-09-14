using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class PurchaseOrderDetailGetModel
    {
        public class PurchaseOrder
        {
            public int PurchaseOrderId { get; set; }
            public string Status { get; set; }
            public string PoNo { get; set; }
            public string ShipVia { get; set; }
            public string VendorName { get; set; }
            public string VendorCity { get; set; }
            public string VendorZip { get; set; }
            public string VendorAddress1 { get; set; }
            public string VendorAddress2 { get; set; }
            public string VendorStateOrProvince { get; set; }
            public string VendorAccount { get; set; }
            public string VendorPhoneNumber { get; set; }
            public string PurchaseOrderNo { get; set; }
            public DateTime OrderDate { get; set; }
            public int OrderQty { get; set; }
            public int Remaining { get; set; }
            public DateTime? LastUpdated { get; set; }
        }

        public class Item
        {
            public int ReceiveId { get; set; }
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string Description { get; set; }
            public string UOM { get; set; }
            public int OrderQty { get; set; }
            public int ReceivedQty { get; set; }
            public int? LotId { get; set; }
            public string LotNo { get; set; }
            public string ExpirationDate { get; set; }
        }

        public PurchaseOrderDetailGetModel()
        {
            Items = new List<Item>();
        }

        public PurchaseOrder PurchaseOrderDetail { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}
