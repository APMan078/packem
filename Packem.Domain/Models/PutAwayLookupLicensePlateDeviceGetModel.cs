using Packem.Domain.Common.Enums;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class PutAwayLookupLicensePlateDeviceGetModel
    {
        public class Item
        {
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string ItemDescription { get; set; }
            public int TotalQty { get; set; }

            //public int LicensePlateItemId { get; set; }
            //public int? ItemId { get; set; }
            //public string ItemSKU { get; set; }
            //public int? VendorId { get; set; }
            //public string VendorName { get; set; }
            //public int? LotId { get; set; }
            //public string LotNo { get; set; }
            //public string ReferenceNo { get; set; }
            //public int? Cases { get; set; }
            //public int? EaCase { get; set; }
            //public int? TotalQty { get; set; }
            //public int? TotalWeight { get; set; }
        }

        public PutAwayLookupLicensePlateDeviceGetModel()
        {
            Items = new List<Item>();
        }

        public int PutAwayId { get; set; }
        public int LicensePlateId { get; set; }
        public string LicensePlateNo { get; set; }
        public LicensePlateTypeEnum LicensePlateType { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}