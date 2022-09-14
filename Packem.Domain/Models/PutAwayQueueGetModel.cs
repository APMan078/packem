using Packem.Domain.Common.Enums;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class PutAwayQueueGetModel
    {
        public class PutAway
        {
            public int PutAwayId { get; set; }
            public int ItemId { get; set; }
            public string SKU { get; set; }
            public string UOM { get; set; }
            public string Description { get; set; }
            public int Qty { get; set; }
            public int Remaining { get; set; }
            //public PutAwayTypeEnum PutAwayType { get; set; }
            public DateTime ReceivedTime { get; set; }
        }

        public PutAwayQueueGetModel()
        {
            PutAways = new List<PutAway>();
        }

        public int Items { get; set; }
        public IEnumerable<PutAway> PutAways { get; set; }
    }
}