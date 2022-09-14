using System;

namespace Packem.Domain.Models
{
    public class LicensePlateHistoryGetModel
    {
        public int LicensePlateId { get; set; }
        public string LicensePlateNo { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Arrival { get; set; }
        public string Location { get; set; }
        public string SKU { get; set; }
        public string Generated { get; set; }
        public string Owner { get; set; }
    }
}