namespace Packem.Domain.Models
{
    public class TransferCreateModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? ItemId { get; set; }
        public int? ItemFacilityId { get; set; }
        public int? ItemBinId { get; set; }
        public int? NewZoneId { get; set; }
        public int? NewBinId { get; set; }
        public int? QtyToTransfer { get; set; }
    }
}