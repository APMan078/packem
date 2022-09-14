namespace Packem.Domain.Models
{
    public class OrderLineUpdateModel
    {
        public int? OrderLineId { get; set; }
        public int? Qty { get; set; }
        public int? PerUnitPrice { get; set; }
    }
}