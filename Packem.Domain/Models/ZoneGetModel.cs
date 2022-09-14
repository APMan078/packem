namespace Packem.Domain.Models
{
    public class ZoneGetModel
    {
        public int ZoneId { get; set; }
        public int CustomerLocationId { get; set; }
        public int CustomerFacilityId { get; set; }
        public string Name { get; set; }
    }
}
