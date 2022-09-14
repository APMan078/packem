using System;

namespace Packem.Domain.Models
{
    public class BinZoneDeviceGetModel
    {
        public int ZoneId { get; set; }
        public string Zone { get; set; }
        public int BinId { get; set; }
        public string Bin { get; set; }
        public int Qty { get; set; }
        public string LotNo { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}