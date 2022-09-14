using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class BinGetModel
    {
        public int BinId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string Name { get; set; }
        public BinCategoryEnum Category { get; set; }
    }
}
