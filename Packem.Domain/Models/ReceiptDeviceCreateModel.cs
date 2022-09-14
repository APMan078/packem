namespace Packem.Domain.Models
{
    public class ReceiptDeviceCreateModel
    {
        public int? ItemId { get; set; }
        public int? Qty { get; set; }
        public LotGetCreateModel NewLotGetCreate { get; set; }
    }
}