namespace Packem.Domain.Models
{
    public class OrderLineBinCreateModel
    {
        public int? UserId { get; set; }
        public int? OrderLineId { get; set; }
        public int? BinId { get; set; }
        public int? Qty { get; set; }
    }
}