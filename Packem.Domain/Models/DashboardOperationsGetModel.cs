namespace Packem.Domain.Models
{
    public class DashboardOperationsGetModel
    {
        public int Operators { get; set; }
        public int OpsManagers { get; set; }
        public int ActiveDevices { get; set; }
        public int RegisteredBins { get; set; }
        public int BinsInUse { get; set; }
        public string Utilization { get; set; }
        public int SalesOrders { get; set; }
        public int SalesOrdersUnits { get; set; }
    }
}