using System;

namespace Packem.Domain.Models
{
    public class ItemGetModel
    {
        public int ItemId { get; set; }
        public int CustomerId { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Threshold { get; set; }
    }
}