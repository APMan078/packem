using System;

namespace Packem.Domain.Models
{
    public class UserTokenModel
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
