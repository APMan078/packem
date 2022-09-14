namespace Packem.Domain.Models
{
    public class OrderLineCreateModel
    {
        public int? SaleOrderId { get; set; }
        public int? ItemId { get; set; }
        public int? Qty { get; set; }
        public int? PerUnitItemPrice { get; set; }
    }
}