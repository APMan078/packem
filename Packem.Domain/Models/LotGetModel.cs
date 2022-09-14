using System;

namespace Packem.Domain.Models
{
    public class LotGetModel
    {
        public int LotId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ItemId { get; set; }
        public string LotNo { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}