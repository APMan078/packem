using System;

namespace Packem.Domain.Models
{
    public class BinLookupItemQuantityGetModel
    {
        public int BinId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public string LotNo { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}