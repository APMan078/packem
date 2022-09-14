using System;

namespace Packem.Domain.Models
{
    public class RecallQueueGetModel
    {
        public int RecallId { get; set; }
        public int ItemId { get; set; }
        public string ItemSKU { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUOM { get; set; }
        public DateTime RecallDate { get; set; }
        public int Qty { get; set; }
        public int Received { get; set; }
    }
}