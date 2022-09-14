using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class User : ISoftDelete
    {
        public User()
        {
            UserRoleVendors = new HashSet<UserRoleVendor>();
            ErrorLogs = new HashSet<ErrorLog>();
            ActivityLogs = new HashSet<ActivityLog>();
            LicensePlates = new HashSet<LicensePlate>();
        }

        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public string Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<UserRoleVendor> UserRoleVendors { get; set; }
        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
        public virtual ICollection<LicensePlate> LicensePlates { get; set; }
    }
}