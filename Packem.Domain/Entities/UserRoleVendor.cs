using Packem.Domain.Common.Interfaces;

namespace Packem.Domain.Entities
{
    public partial class UserRoleVendor : ISoftDelete
    {
        public int UserRoleVendorId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public int? VendorId { get; set; }
        public bool Deleted { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}