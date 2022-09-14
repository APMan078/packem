using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class OrderCustomer : ISoftDelete
    {
        public OrderCustomer()
        {
            SaleOrders = new HashSet<SaleOrder>();
            OrderCustomerAddresses = new HashSet<OrderCustomerAddress>();
        }

        public int OrderCustomerId { get; set; }
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public OrderCustomerPaymentTypeEnum PaymentType { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<SaleOrder> SaleOrders { get; set; }
        public virtual ICollection<OrderCustomerAddress> OrderCustomerAddresses { get; set; }
    }
}