namespace Packem.Domain.Models
{
    public class OrderCustomerManagementGetModel
    {
        public int OrderCustomerId { get; set; }
        public string CustomerName { get; set; }
        public int? NoShippingAddresses { get; set; }
        public int? NoBillingAddresses { get; set; }
    }
}