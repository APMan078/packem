namespace Packem.Domain.Models
{
    public class OrderLineGetModel
    {
        public int OrderLineId { get; set; }
        public int CustomerLocationId { get; set; }
        public int SaleOrderId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public int Received { get; set; }
        public int Remaining { get; set; }
        public int? PerUnitItemPrice { get; set; }
    }
}