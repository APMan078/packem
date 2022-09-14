using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class ZoneAndBinImportModel
    {
        public string BinName { get; set; }
        public string ZoneName { get; set; }
        public string BinZone { get; set; }
        public BinCategoryEnum Category { get; set; }
        public int CustomerLocationId { get; set; }
        public int CustomerFacilityId { get; set; }
    }
}

