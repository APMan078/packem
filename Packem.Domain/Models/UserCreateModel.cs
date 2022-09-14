using Packem.Domain.Common.Enums;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class UserCreateModel
    {
        public UserCreateModel()
        {
            VendorIds = new List<int>();
        }

        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public RoleEnum? Role { get; set; }
        public string Password { get; set; }
        public IEnumerable<int> VendorIds { get; set; } // For RoleId of 5 only, Viewer
    }
}