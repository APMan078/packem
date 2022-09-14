namespace Packem.Domain.Models
{
    public class ItemLookupDeviceGetModel
    {
        public int ItemId { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public string Barcode { get; set; }
        public int QtyOnHand { get; set; }
        public int OnOrder { get; set; }
    }
}