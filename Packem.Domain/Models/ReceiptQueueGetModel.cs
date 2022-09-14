using System;
using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class ReceiptQueueGetModel
    {
        public class Location
        {
            public int ZoneId { get; set; }
            public string Zone { get; set; }
            public int BinId { get; set; }
            public string Bin { get; set; }
            public int Qty { get; set; }
            public DateTime ReceivedDateTime { get; set; }
        }

        public class Receipt
        {
            public int ReceiptId { get; set; }
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string ItemDescription { get; set; }
            public string ItemUOM { get; set; }
            public DateTime ReceiptDate { get; set; }
            public int Qty { get; set; }
            public int Remaining { get; set; }

            public Receipt()
            {
                Locations = new List<Location>();
            }

            public IEnumerable<Location> Locations { get; set; }
        }

        public ReceiptQueueGetModel()
        {
            Receipts = new List<Receipt>();
        }

        public IEnumerable<Receipt> Receipts { get; set; }
    }
}