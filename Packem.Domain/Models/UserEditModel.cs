using Packem.Domain.Common.Enums;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class UserEditModel
    {
        public UserEditModel()
        {
            VendorIds = new List<int>();
        }

        public int UserId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public RoleEnum? Role { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<int> VendorIds { get; set; } // For RoleId of 5 only, Viewer
    }
}