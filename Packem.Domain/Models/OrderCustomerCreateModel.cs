using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class OrderCustomerCreateModel
    {
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public OrderCustomerPaymentTypeEnum? PaymentType { get; set; }
    }
}