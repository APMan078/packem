using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class CustomerDevice : ISoftDelete
    {
        public CustomerDevice()
        {
            CustomerDeviceTokens = new HashSet<CustomerDeviceToken>();
        }
        public int CustomerDeviceId { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime AddedDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivedDateTime { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual ICollection<CustomerDeviceToken> CustomerDeviceTokens { get; set; }
    }
}
