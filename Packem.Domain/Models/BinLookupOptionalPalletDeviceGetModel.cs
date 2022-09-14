namespace Packem.Domain.Models
{
    public class BinLookupOptionalPalletDeviceGetModel
    {
        public int BinId { get; set; }
        public string Name { get; set; }
        public int? PalletCount { get; set; }
        public int? EachCount { get; set; }
        public int? PalletQty { get; set; }
        public int? EachQty { get; set; }
        public int? TotalQty { get; set; }
    }
}