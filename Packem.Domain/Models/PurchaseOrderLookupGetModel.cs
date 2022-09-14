using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class PurchaseOrderLookupGetModel
    {
        public class PurchaseOrder
        {
            public int PurchaseOrderId { get; set; }
            public string Status { get; set; }
            public string PoNo { get; set; }
            public string ShipVia { get; set; }
            public string VendorName { get; set; }
            public string VendorCity { get; set; }
            public string VendorAccount { get; set; }
            public DateTime OrderDate { get; set; }
            public int OrderQty { get; set; }
            public int Remaining { get; set; }
            public DateTime? LastUpdated { get; set; }
        }

        public PurchaseOrderLookupGetModel()
        {
            PurchaseOrders = new List<PurchaseOrder>();
        }

        public int PurchaseOrderCount { get; set; }
        public int ExpectedUnits { get; set; }
        public int ReceiptsToday { get; set; }
        public IEnumerable<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
