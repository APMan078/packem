namespace Packem.Domain.Models
{
    public class ReceiptCreateModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ItemId { get; set; }
        public int? Qty { get; set; }
        public int? LotId { get; set; }
    }
}