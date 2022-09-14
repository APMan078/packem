using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class SaleOrderPickingStatusUpdateModel
    {
        public int? SaleOrderId { get; set; }
        public PickingStatusEnum? PickingStatus { get; set; }
    }
}