﻿using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class OrderCustomerAddressCreateModel
    {
        public int? OrderCustomerId { get; set; }
        public OrderCustomerAddressType? AddressType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }
}