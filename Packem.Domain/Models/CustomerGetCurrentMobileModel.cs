using Packem.Domain.Common.Enums;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class CustomerGetCurrentMobileModel
    {
        public class CurrentCustomerLocation
        {
            public int CustomerLocationId { get; set; }
            public string Name { get; set; }
            //public StateEnum State { get; set; }
            public IEnumerable<CustomerFacility> CustomerFacilities { get; set; }

            public CurrentCustomerLocation()
            {
                CustomerFacilities = new List<CustomerFacility>();
            }
        }

        public class CustomerFacility
        {
            public int CustomerFacilityId { get; set; }
            public string Name { get; set; }
        }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public CurrentCustomerLocation CustomerLocation { get; set; }
    }
}
