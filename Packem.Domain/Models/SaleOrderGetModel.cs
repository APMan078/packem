using Packem.Domain.Common.Enums;
using System;

namespace Packem.Domain.Models
{
    public class SaleOrderGetModel
    {
        public int SaleOrderId { get; set; }
        public int CustomerLocationId { get; set; }
        public int CustomerFacilityId { get; set; }
        public string SaleOrderNo { get; set; }
        public SaleOrderStatusEnum Status { get; set; }
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
        public string ShipToCity { get; set; }
        public string ShipToCountry { get; set; }
        public string ShipToPhoneNumber { get; set; }
        public string BillToAddress1 { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToStateProvince { get; set; }
        public string BillToZipPostalCode { get; set; }
        public string BillToCity { get; set; }
        public string BillToCountry { get; set; }
        public string BillToPhoneNumber { get; set; }
        public PickingStatusEnum PickingStatus { get; set; }
    }
}