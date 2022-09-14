﻿namespace Packem.Domain.Models
{
    public class VendorCreateModel
    {
        public int? CustomerId { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
    }
}
