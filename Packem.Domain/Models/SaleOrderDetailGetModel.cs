using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class SaleOrderDetailGetModel
    {
        public class OrderDetail
        {
            public int SaleOrderId { get; set; }
            public string OrderNo { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? PromiseDate { get; set; }
            public DateTime? FulfilledDate { get; set; }
            public int OrderCustomerId { get; set; }
            public string CustomerName { get; set; }
            public int? ItemId { get; set; }
            public string ItemSKU { get; set; }
            public int QtyOrdered { get; set; }
            public decimal? TotalSalePrice { get; set; }
            public string QtyOnHand { get; set; }
            public string ShipToAddress1 { get; set; }
            public string ShipToAddress2 { get; set; }
            public string ShipToCity { get; set; }
            public string ShipToStateProvince { get; set; }
            public string ShipToZipPostalCode { get; set; }
            public string ShipToCountry { get; set; }
            public string ShipToPhoneNumber { get; set; }
            public string BillToAddress1 { get; set; }
            public string BillToAddress2 { get; set; }
            public string BillToCity { get; set; }
            public string BillToStateProvince { get; set; }
            public string BillToZipPostalCode { get; set; }
            public string BillToCountry { get; set; }
            public string BillToPhoneNumber { get; set; }
        }

        public class OrderLine
        {
            public int OrderLineId { get; set; }
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string Description { get; set; }
            public string UOM { get; set; }
            public int OrderQty { get; set; }
            public int? ZoneId { get; set; }
            public string Zone { get; set; }
            public int BinLocations { get; set; }
            public int? PerUnitPrice { get; set; }
        }

        public SaleOrderDetailGetModel()
        {
            OrderLines = new List<OrderLine>();
        }

        public OrderDetail OrderDetails { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
    }
}