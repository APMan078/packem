using Packem.Domain.Common.Enums;
using System;

namespace Packem.Domain.Models
{
    public class PurchaseOrderGetModel
    {
        public int PurchaseOrderId { get; set; }
        public int CustomerLocationId { get; set; }
        public int CustomerFacilityId { get; set; }
        public int VendorId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public PurchaseOrderStatusEnum Status { get; set; }
        public string ShipVia { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderQty { get; set; }
        public int Remaining { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        
    }
}
