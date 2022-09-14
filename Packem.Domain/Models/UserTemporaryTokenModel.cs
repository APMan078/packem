using System;
namespace Packem.Domain.Models
{
    public class UserTemporaryTokenModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}

