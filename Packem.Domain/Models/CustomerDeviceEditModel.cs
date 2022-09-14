namespace Packem.Domain.Models
{
    public class CustomerDeviceEditModel
    {
        public int? CustomerDeviceId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string SerialNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
