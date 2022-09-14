using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class CustomerLocationCreateModel
    {
        public int? CustomerId { get; set; } // only need if SuperAdmin, else it will use the user's CompanyId
        public string Name { get; set; }
        //public StateEnum State { get; set; }
    }
}
