using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class PurchaseOrder : ISoftDelete
    {
        public PurchaseOrder()
        {
            Receives = new HashSet<Receive>();
        }

        public int PurchaseOrderId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public int? VendorId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public PurchaseOrderStatusEnum Status { get; set; }
        public string ShipVia { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderQty { get; set; }
        public int Remaining { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual CustomerFacility CustomerFacility { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<Receive> Receives { get; set; }
        public DateTime? StatusUpdatedDateTime { get; set; }
    }
}