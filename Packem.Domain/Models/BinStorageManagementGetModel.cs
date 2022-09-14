using System.Collections.Generic;
using Packem.Domain.Common.Enums;
namespace Packem.Domain.Models
{
    public class BinStorageManagementGetModel
    {
        public class BinDetail
        {
            public int BinId { get; set; }
            public string Name { get; set; }
            public BinCategoryEnum Category { get; set; }
            public string Zone { get; set; }
            public string UOM { get; set; }
            public int UniqueSKU { get; set; }
            public int Qty { get; set; }
        }

        public BinStorageManagementGetModel()
        {
            BinDetails = new List<BinDetail>();
        }

        public int Bins { get; set; }
        public int Zones { get; set; }
        public IEnumerable<BinDetail> BinDetails { get; set; }
    }
}