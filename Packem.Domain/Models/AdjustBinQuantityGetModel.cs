using System;

namespace Packem.Domain.Models
{
    public class AdjustBinQuantityGetModel
    {
        public int AdjustBinQuantityId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ItemId { get; set; }
        public int BinId { get; set; }
        public int OldQty { get; set; }
        public int NewQty { get; set; }
        public DateTime AdjustDateTime { get; set; }
    }
}