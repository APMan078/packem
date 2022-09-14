using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class LicensePlateGetModel
    {
        public int LicensePlateId { get; set; }
        public int CustomerId { get; set; }
        public string LicensePlateNo { get; set; }
        public LicensePlateTypeEnum LicensePlateType { get; set; }
    }
}