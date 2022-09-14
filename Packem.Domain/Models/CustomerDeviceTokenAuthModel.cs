namespace Packem.Domain.Models
{
    public class CustomerDeviceTokenAuthModel
    {
        public int CustomerDeviceTokenId { get; set; }
        public string DeviceToken { get; set; }
        public int CustomerId { get; set; }
        public int CustomerLocationId { get; set; }
    }
}