using Packem.Domain.Common.Enums;
using System;

namespace Packem.Domain.Models
{
    public class RecallQueueLookupGetModel
    {
        public int RecallId { get; set; }
        public DateTime RecallDate { get; set; }
        public RecallStatusEnum Status { get; set; }
        public string ItemSKU { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUOM { get; set; }
        public int Bins { get; set; }
    }
}