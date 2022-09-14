namespace Packem.Domain.Models
{
    public class RecallBinCreateModel
    {
        public int? UserId { get; set; }
        public int? RecallId { get; set; }
        public int? BinId { get; set; }
        public int? Qty { get; set; }
    }
}