namespace Packem.Domain.Models
{
    public class UserGetModel
    {
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
