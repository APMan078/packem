using System;
namespace Packem.Domain.Models
{
    public class ResetPasswordRequestCreateModel
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
}

