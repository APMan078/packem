namespace Packem.Domain.Models
{
    public class ItemVendorGetModel
    {
        public int ItemId { get; set; }
        public string ItemSKU { get; set; }
        public string Facility { get; set; }
        public string Zone { get; set; }
        public int BinId { get; set; }
        public string Bin { get; set; }
        public int QtyOnHand { get; set; }
        public string Description { get; set; }
        public int Vendors { get; set; }
    }
}
