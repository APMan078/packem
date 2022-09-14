using System;

namespace Packem.Domain.Models
{
    public class PutAwayBinGetModel
    {
        public int PutAwayBinId { get; set; }
        public int CustomerLocationId { get; set; }
        public int PutAwayId { get; set; }
        public int BinId { get; set; }
        public int Qty { get; set; }
        public DateTime ReceivedDateTime { get; set; }
    }
}