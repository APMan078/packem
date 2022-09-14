namespace Packem.Domain.Models
{
    public class CustomerDeviceCreateModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string SerialNumber { get; set; }
    }
}
