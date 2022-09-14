using System.ComponentModel;

namespace Packem.Domain.Common.Enums
{
    public enum OrderCustomerAddressType
    {
        [Description("Shipping Address")]
        ShippingAddress = 1,
        [Description("Billing Address")]
        BillingAddress = 2
    }
}