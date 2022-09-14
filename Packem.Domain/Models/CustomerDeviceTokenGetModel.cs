using System;

namespace Packem.Domain.Models
{
    public class CustomerDeviceTokenGetModel
    {
        public int CustomerDeviceTokenId { get; set; }
        public int CustomerDeviceId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime AddedDateTime { get; set; }
        public bool IsValidated { get; set; }
        public DateTime? ValidatedDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivedDateTime { get; set; }
    }
}
