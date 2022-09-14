using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
namespace Packem.Domain.Entities
{
    public partial class Item : ISoftDelete
    {
        public Item()
        {
            ItemVendors = new HashSet<ItemVendor>();
            Receives = new HashSet<Receive>();
            Receipts = new HashSet<Receipt>();
            Transfers = new HashSet<Transfer>();
            AdjustBinQuantities = new HashSet<AdjustBinQuantity>();
            OrderLines = new HashSet<OrderLine>();
            Recalls = new HashSet<Recall>();
            Lots = new HashSet<Lot>();
            LicensePlateItems = new HashSet<LicensePlateItem>();
        }

        public int ItemId { get; set; }
        public int? CustomerId { get; set; }
        //public string ItemNo { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Threshold { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public string Barcode { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual UnitOfMeasure UnitOfMeasure { get; set; }
        public virtual ICollection<ItemVendor> ItemVendors { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual ICollection<Receive> Receives { get; set; }
        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
        public virtual ICollection<AdjustBinQuantity> AdjustBinQuantities { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<Recall> Recalls { get; set; }
        public virtual ICollection<Lot> Lots { get; set; }
        public virtual ICollection<LicensePlateItem> LicensePlateItems { get; set; }
    }
}