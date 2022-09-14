using System;

namespace Packem.Domain.Models
{
    public class LotLookupGetModel
    {
        public int LotId { get; set; }
        public string LotNo { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}