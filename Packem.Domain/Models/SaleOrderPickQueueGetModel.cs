using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class SaleOrderPickQueueGetModel
    {
        public class Item
        {
            public int SaleOrderId { get; set; }
            public string SaleOrderNo { get; set; }
            public string PickingStatus { get; set; }
            public int Units { get; set; }
            public int Locations { get; set; }
        }

        public SaleOrderPickQueueGetModel()
        {
            Items = new List<Item>();
        }

        public int ItemCount { get; set; }
        public int CompletedToday { get; set; }
        public int UnitsPending { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}