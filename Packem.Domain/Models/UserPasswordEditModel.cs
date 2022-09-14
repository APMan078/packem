namespace Packem.Domain.Models
{
    public class UserPasswordEditModel
    {
        public int? UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}