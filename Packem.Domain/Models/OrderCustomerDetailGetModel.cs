using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class OrderCustomerDetailGetModel
    {
        public class CustomerDetail
        {
            public int OrderCustomerId { get; set; }
            public string CustomerName { get; set; }
            public string ShipToAddress1 { get; set; }
            public string ShipToAddress2 { get; set; }
            public string City { get; set; }
            public string ShipToStateProvince { get; set; }
            public string ShipToZipPostalCode { get; set; }
            public string ShipToCountry { get; set; }
            public string ShipToPhoneNumber { get; set; }
            public string BillToAddress1 { get; set; }
            public string BillToAddress2 { get; set; }
            public string BillToStateProvince { get; set; }
            public string BillToZipPostalCode { get; set; }
            public string BillToCountry { get; set; }
            public string BillToPhoneNumber { get; set; }
        }

        public class SaleOrder
        {
            public int SaleOrderId { get; set; }
            public string OrderNo { get; set; }
            public string Status { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? PromisedDate { get; set; }
            public int OrderQty { get; set; }
        }

        public class ItemOrder
        {
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string Description { get; set; }
            public string UOM { get; set; }
            public int OrderQty { get; set; }
            public int QtyOnHand { get; set; }
            public int BinLocations { get; set; }
            public int? PerUnitPrice { get; set; }
        }

        public OrderCustomerDetailGetModel()
        {
            CustomerDetails = new List<CustomerDetail>();
            SaleOrders = new List<SaleOrder>();
            ItemOrders = new List<ItemOrder>();
        }

        public int Orders { get; set; }
        public int UniqueItemsOrdered { get; set; }
        public IEnumerable<CustomerDetail> CustomerDetails { get; set; }
        public IEnumerable<SaleOrder> SaleOrders { get; set; }
        public IEnumerable<ItemOrder> ItemOrders { get; set; }
    }
}