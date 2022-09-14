using Packem.Domain.Common.Interfaces;
using System;

namespace Packem.Domain.Entities
{
    public partial class CustomerDeviceToken : ISoftDelete
    {
        public int CustomerDeviceTokenId { get; set; }
        public int? CustomerDeviceId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime AddedDateTime { get; set; }
        public bool IsValidated { get; set; }
        public DateTime? ValidatedDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivedDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerDevice CustomerDevice { get; set; }
    }
}
