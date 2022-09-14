namespace Packem.Domain.Models
{
    public class AdjustBinQuantityCreateModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ItemId { get; set; }
        public int? BinId { get; set; }
        public int? NewQty { get; set; }
    }
}