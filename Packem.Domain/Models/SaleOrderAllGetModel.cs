using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class SaleOrderAllGetModel
    {
        public class SaleOrder
        {
            public int SaleOrderId { get; set; }
            public string Status { get; set; }
            public string OrderNo { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? PromiseDate { get; set; }
            public DateTime? FulfilledDate { get; set; }
            public int OrderCustomerId { get; set; }
            public string CustomerNo { get; set; }
            public int? ItemId { get; set; }
            public string ItemSKU { get; set; }
            public int QtyOrdered { get; set; }
            public decimal? TotalSalePrice { get; set; }
            public string QtyOnHand { get; set; }
            public string CustomerName { get; set; }
        }

        public SaleOrderAllGetModel()
        {
            SaleOrders = new List<SaleOrder>();
        }

        public int PendingOrders { get; set; }
        public int InPicking { get; set; }
        //public int UnitToShip { get; set; }
        //public int ItemsInLowStock { get; set; }
        public IEnumerable<SaleOrder> SaleOrders { get; set; }
    }
}