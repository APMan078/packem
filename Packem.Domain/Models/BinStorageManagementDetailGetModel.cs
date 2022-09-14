using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class BinStorageManagementDetailGetModel
    {
        public class Bin
        {
            public int BinId { get; set; }
            public string Name { get; set; }
            public string Zone { get; set; }
            public string UOM { get; set; }
            public int UniqueSKU { get; set; }
        }

        public class Item
        {
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string Description { get; set; }
            public string UOM { get; set; }
            public int Qty { get; set; }
        }

        public BinStorageManagementDetailGetModel()
        {
            ItemDetails = new List<Item>();
        }

        public int Items { get; set; }
        public int UniqueSKUs { get; set; }
        public Bin BinDetail { get; set; }
        public IEnumerable<Item> ItemDetails { get; set; }
    }
}