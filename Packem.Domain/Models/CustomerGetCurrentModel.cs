using Packem.Domain.Common.Enums;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class CustomerGetCurrentModel
    {
        public class Customer
        {
            public int CustomerId { get; set; }
            public string Name { get; set; }
            public IEnumerable<CustomerLocation> CustomerLocations { get; set; }

            public Customer()
            {
                CustomerLocations = new List<CustomerLocation>();
            }
        }

        public class CustomerLocation
        {
            public int CustomerLocationId { get; set; }
            public string Name { get; set; }
            //public StateEnum State { get; set; }
            public IEnumerable<CustomerFacility> CustomerFacilities { get; set; }

            public CustomerLocation()
            {
                CustomerFacilities = new List<CustomerFacility>();
            }
        }

        public class CustomerFacility
        {
            public int CustomerFacilityId { get; set; }
            public string Name { get; set; }
        }

        public IEnumerable<Customer> Customers { get; set; }

        public CustomerGetCurrentModel()
        {
            Customers = new List<Customer>();
        }
    }
}
