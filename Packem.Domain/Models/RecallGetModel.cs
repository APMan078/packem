using Packem.Domain.Common.Enums;
using System;

namespace Packem.Domain.Models
{
    public class RecallGetModel
    {
        public int RecallId { get; set; }
        public int CustomerLocationId { get; set; }
        public int CustomerFacilityId { get; set; }
        public int ItemId { get; set; }
        public DateTime RecallDate { get; set; }
        public RecallStatusEnum Status { get; set; }
    }
}