namespace Packem.Domain.Models
{
    public class InventoryGetModel
    {
        public int InventoryId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ItemId { get; set; }
        public int QtyOnHand { get; set; }
    }
}
