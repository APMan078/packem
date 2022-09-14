using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class LicensePlateLookupDeviceGetModel
    {
        public int LicensePlateId { get; set; }
        public string LicensePlateNo { get; set; }
        public LicensePlateTypeEnum LicensePlateType { get; set; }
    }
}