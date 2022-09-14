using System;

namespace Packem.Domain.Models
{
    public class ReceiptGetModel
    {
        public int ReceiptId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public int Remaining { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int? LotId { get; set; }
    }
}