using System;

namespace Packem.Domain.Models
{
    public class ItemExpirationDateUpdateModel
    {
        public int? ItemId { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}