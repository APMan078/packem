namespace Packem.Domain.Models
{
    public class PutAwayBinCreateModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? UserId { get; set; }
        public int? PutAwayId { get; set; }
        public int? BinId { get; set; }
        public int? Qty { get; set; }
    }
}