using System;

namespace Packem.Domain.Models
{
    public class ReceiveGetModel
    {
        public int ReceiveId { get; set; }
        public int CustomerLocationId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public int Received { get; set; }
        public int Remaining { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public int? LotId { get; set; }
    }
}