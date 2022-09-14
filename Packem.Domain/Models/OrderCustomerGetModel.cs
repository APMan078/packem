using Packem.Domain.Common.Enums;
using System;

namespace Packem.Domain.Models
{
    public class OrderCustomerGetModel
    {
        public int OrderCustomerId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public OrderCustomerPaymentTypeEnum PaymentType { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}