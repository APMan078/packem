using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class OrderLine : ISoftDelete
    {
        public OrderLine()
        {
            OrderLineBins = new HashSet<OrderLineBin>();
        }

        public int OrderLineId { get; set; }
        public int? CustomerLocationId { get; set; }
        public int? SaleOrderId { get; set; }
        public int? ItemId { get; set; }
        public int? PerUnitPrice { get; set; }
        public int Qty { get; set; }
        public int Received { get; set; }
        public int Remaining { get; set; }
        public bool Deleted { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual SaleOrder SaleOrder { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<OrderLineBin> OrderLineBins { get; set; }
    }
}