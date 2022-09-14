using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class OrderLinePickLookupGetModel
    {
        public class Bin
        {
            public int BinId { get; set; }
            public string Name { get; set; }
        }

        public OrderLinePickLookupGetModel()
        {
            Bins = new List<Bin>();
        }

        public int OrderLineId { get; set; }
        public int ItemId { get; set; }
        public string ItemSKU { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUOM { get; set; }
        public int Qty { get; set; }
        public int Received { get; set; }
        public int Remaining { get; set; }
        public IEnumerable<Bin> Bins { get; set; }
    }
}