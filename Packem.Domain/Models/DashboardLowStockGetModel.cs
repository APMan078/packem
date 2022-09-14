namespace Packem.Domain.Models
{
    public class DashboardLowStockGetModel
    {
        public int ItemId { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public int Expect { get; set; }
        public int OnHand { get; set; }
        public int Sold { get; set; }
        public int Backorder { get; set; }
    }
}