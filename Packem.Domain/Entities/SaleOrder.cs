using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class SaleOrder : ISoftDelete
    {
        public SaleOrder()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int SaleOrderId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public string SaleOrderNo { get; set; } // unique
        public SaleOrderStatusEnum Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PromiseDate { get; set; }
        public DateTime? FulfilledDate { get; set; } // datetime, name should be FulfilledDateTime
        public int OrderQty { get; set; }
        public int Remaining { get; set; }
        public decimal? TotalSalePrice { get; set; }
        public int? OrderCustomerId { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public PickingStatusEnum PickingStatus { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual CustomerFacility CustomerFacility { get; set; }
        public virtual OrderCustomer OrderCustomer { get; set; }
        public virtual OrderCustomerAddress ShippingAddress { get; set; }
        public virtual OrderCustomerAddress BillingAddress { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}