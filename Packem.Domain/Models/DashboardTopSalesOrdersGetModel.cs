namespace Packem.Domain.Models
{
    public class DashboardTopSalesOrdersGetModel
    {
        public int SaleOrderId { get; set; }
        public string SaleOrderNo { get; set; }
        public string Customer { get; set; }
        public int Total { get; set; }
        public int Units { get; set; }
        public int SKUs { get; set; }
    }
}