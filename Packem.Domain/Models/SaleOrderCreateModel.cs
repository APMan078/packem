using System;

namespace Packem.Domain.Models
{
    public class SaleOrderCreateModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public string SaleOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? PromiseDate { get; set; } // not required
        public int? OrderCustomerId { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
    }
}