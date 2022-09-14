using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class AppState
    {
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int? CustomerLocationId { get; set; }
        public RoleEnum Role { get; set; }
    }
}
