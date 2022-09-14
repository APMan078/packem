using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class OrderCustomerAddress : ISoftDelete
    {
        public OrderCustomerAddress()
        {
            SaleOrderShippingAddresses = new HashSet<SaleOrder>();
            SaleOrderBillingAddresses = new HashSet<SaleOrder>();
        }

        public int OrderCustomerAddressId { get; set; }
        public OrderCustomerAddressType AddressType { get; set; }
        public int? OrderCustomerId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string ZipPostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public bool Deleted { get; set; }

        public virtual OrderCustomer OrderCustomer { get; set; }
        public virtual ICollection<SaleOrder> SaleOrderShippingAddresses { get; set; }
        public virtual ICollection<SaleOrder> SaleOrderBillingAddresses { get; set; }
    }
}