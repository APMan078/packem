namespace Packem.Domain.Models
{
    public class RecallCreateModel
    {
        public int? CustomerId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public int? ItemId { get; set; }
    }
}