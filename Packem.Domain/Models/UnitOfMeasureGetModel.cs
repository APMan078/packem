using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class UnitOfMeasureGetModel
    {
        public int UnitOfMeasureId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public UnitOfMeasureTypeEnum Type { get; set; }
    }
}