using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class CustomerLocationGetModel
    {
        public int CustomerLocationId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        //public StateEnum State { get; set; }
    }
}
