using System;

namespace Packem.Domain.Models
{
    public class OrderLineBinGetModel
    {
        public int OrderLineBinId { get; set; }
        public int CustomerLocationId { get; set; }
        public int OrderLineId { get; set; }
        public int BinId { get; set; }
        public int Qty { get; set; }
        public DateTime PickDateTime { get; set; }
    }
}