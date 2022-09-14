using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class PutAwayBinDeviceCreateModel
    {
        public int? UserId { get; set; }
        public int? PutAwayId { get; set; }
        public BinGetCreateModel BinGetCreate { get; set; }
        public int? Qty { get; set; }
        public PutAwayTypeEnum? PutAwayType { get; set; }
    }
}