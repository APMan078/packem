namespace Packem.Domain.Models
{
    public class DashboardQueuesGetModel
    {
        public int Expected { get; set; }
        public int PurchaseOrders { get; set; }
        public int PutAway { get; set; }
        public int Pick { get; set; }
        public int Transfer { get; set; }
    }
}