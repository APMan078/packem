using System;

namespace Packem.Domain.Models
{
    public class RecallBinGetModel
    {
        public int RecallBinId { get; set; }
        public int CustomerLocationId { get; set; }
        public int RecallId { get; set; }
        public int BinId { get; set; }
        public int Qty { get; set; }
        public DateTime PickDateTime { get; set; }
    }
}