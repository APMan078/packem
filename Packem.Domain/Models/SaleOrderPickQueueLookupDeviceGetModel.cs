using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class SaleOrderPickQueueLookupDeviceGetModel
    {
        public int SaleOrderId { get; set; }
        public string SaleOrderNo { get; set; }
        public PickingStatusEnum PickingStatus { get; set; }
        public int Items { get; set; }
        public int Bins { get; set; }
    }
}