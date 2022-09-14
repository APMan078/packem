using Packem.Domain.Common.Enums;
using System;

namespace Packem.Domain.Models
{
    public class PutAwayLookupDeviceGetModel
    {
        public int PutAwayId { get; set; }
        public int ItemId { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public int Remaining { get; set; }
        public PutAwayTypeEnum PutAwayType { get; set; }
        public string LotNo { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}