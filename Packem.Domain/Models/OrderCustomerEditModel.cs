using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class OrderCustomerEditModel
    {
        public int? OrderCustomerId { get; set; }
        public string Name { get; set; }
        public OrderCustomerPaymentTypeEnum? PaymentType { get; set; }
    }
}