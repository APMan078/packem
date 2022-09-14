using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class SaleOrderPrintGetModel
    {
        public class SaleOrder
        {
            public int SaleOrderId { get; set; }
            public string SaleOrderNo { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? PromiseDate { get; set; }
            public DateTime? FulfilledDate { get; set; }
            public int OrderQty { get; set; }
            public decimal? TotalSalePrice { get; set; }
            public int OrderCustomerId { get; set; }
            public string CustomerName { get; set; }
            public string ShipToAddress1 { get; set; }
            public string ShipToAddress2 { get; set; }
            public string ShipToStateProvince { get; set; }
            public string ShipToZipPostalCode { get; set; }
            public string ShipToCountry { get; set; }
            public string ShipToCity { get; set; }
            public string ShipToPhoneNumber { get; set; }
            public string BillToAddress1 { get; set; }
            public string BillToAddress2 { get; set; }
            public string BillToStateProvince { get; set; }
            public string BillToZipPostalCode { get; set; }
            public string BillToCountry { get; set; }
            public string BillToCity { get; set; }
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

            public OrderLine()
            {
                Zones = new List<Zone>();
            }

            public IEnumerable<Zone> Zones { get; set; }
        }

        public class Zone
        {
            public int ZoneId { get; set; }
            public string Name { get; set; }

            public Zone()
            {
                Bins = new List<Bin>();
            }

            public IEnumerable<Bin> Bins { get; set; }
        }

        public class Bin
        {
            public int BinId { get; set; }
            public string Name { get; set; }
        }

        public SaleOrderPrintGetModel()
        {
            OrderLines = new List<OrderLine>();
        }

        public SaleOrder OrderDetail { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
    }
}