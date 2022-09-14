namespace Packem.Domain.Models
{
    public class InventoryCreateModel
    {
        public int? ItemId { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? FacilityId { get; set; }
        public int? ZoneId { get; set; }
        public int? VendorId { get; set; }
        public string VendorNo { get; set; }
        public string VendorName { get; set; }
        public string VendorPointOfContact { get; set; }
        public string VendorAddress { get; set; }
        public string VendorPhoneNumber { get; set; }
        public int? BinId { get; set; }
        public string BinLocation { get; set; }
        public int? QtyAtBin { get; set; }
        public int? LotId { get; set; }
    }
}