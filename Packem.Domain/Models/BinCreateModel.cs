using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class BinCreateModel
    {
        public int? CustomerLocationId { get; set; }
        public int? ZoneId { get; set; }
        public string Name { get; set; }
        public BinCategoryEnum Category { get; set; }
    }
}
