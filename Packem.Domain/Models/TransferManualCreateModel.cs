namespace Packem.Domain.Models
{
    public class TransferManualCreateModel
    {
        public int? UserId { get; set; }
        public int? ItemId { get; set; }
        public int? ItemFacilityId { get; set; }
        public int? ItemZoneId { get; set; }
        public int? ItemBinId { get; set; }
        public BinGetCreateModel NewBinGetCreate { get; set; }
        public int? QtyToTransfer { get; set; }
    }
}