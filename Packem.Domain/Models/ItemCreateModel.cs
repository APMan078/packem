namespace Packem.Domain.Models
{
    public class ItemCreateModel
    {
        public int? CustomerId { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public int? UOMId { get; set; }
    }
}