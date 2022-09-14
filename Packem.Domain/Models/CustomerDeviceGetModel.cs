using System;

namespace Packem.Domain.Models
{
    public class CustomerDeviceGetModel
    {
        public int CustomerId { get; set; }
        public int CustomerDeviceId { get; set; }
        public int CustomerLocationId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime AddedDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
    }
}
