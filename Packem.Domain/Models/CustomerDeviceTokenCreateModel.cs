namespace Packem.Domain.Models
{
    public class CustomerDeviceTokenCreateModel
    {
        public int? CustomerDeviceId { get; set; }
        public string DeviceToken { get; set; }
    }
}
