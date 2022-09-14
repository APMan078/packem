using System;
using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class SalesOrderImportModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public string SaleOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? PromiseDate { get; set; } // not required
        public DateTime? FulfilledDate { get; set; } // not required
        public string CustomerName { get; set; }
        public int? OrderCustomerId { get; set; }
        public OrderCustomerAddressType? AddressType1 { get; set; }
        public OrderCustomerAddressType? AddressType2 { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public PickingStatusEnum PickingStatus { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingStateProvince { get; set; }
        public string ShippingZipPostalCode { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingPhoneNumber { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingStateProvince { get; set; }
        public string BillingZipPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPhoneNumber { get; set; }
    }
}

