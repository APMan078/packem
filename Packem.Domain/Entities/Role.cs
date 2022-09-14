using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class Role : ISoftDelete
    {
        public Role()
        {
            Users = new HashSet<User>();
            UserRoleVendors = new HashSet<UserRoleVendor>();
        }

        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserRoleVendor> UserRoleVendors { get; set; }
    }
}