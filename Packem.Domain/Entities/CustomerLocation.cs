using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class CustomerLocation : ISoftDelete
    {
        // ActivityLogs
        // CustomerDeviceTokens
        // ErrorLogs
        public CustomerLocation()
        {
            CustomerFacilities = new HashSet<CustomerFacility>();
            CustomerDevices = new HashSet<CustomerDevice>();
            Users = new HashSet<User>();
            Zones = new HashSet<Zone>();
            InventoryZones = new HashSet<InventoryZone>();
            Bins = new HashSet<Bin>();
            InventoryBins = new HashSet<InventoryBin>();
            PurchaseOrders = new HashSet<PurchaseOrder>();
            Receives = new HashSet<Receive>();
            PutAways = new HashSet<PutAway>();
            PutAwayBins = new HashSet<PutAwayBin>();
            Receipts = new HashSet<Receipt>();
            Transfers = new HashSet<Transfer>();
            TransferCurrents = new HashSet<TransferCurrent>();
            TransferNews = new HashSet<TransferNew>();
            AdjustBinQuantities = new HashSet<AdjustBinQuantity>();
            TransferZoneBins = new HashSet<TransferZoneBin>();
            SaleOrders = new HashSet<SaleOrder>();
            OrderLines = new HashSet<OrderLine>();
            OrderLineBins = new HashSet<OrderLineBin>();
            Recalls = new HashSet<Recall>();
            RecallBins = new HashSet<RecallBin>();
            Lots = new HashSet<Lot>();
            Pallets = new HashSet<Pallet>();
            PalletInventories = new HashSet<PalletInventory>();
        }

        public int CustomerLocationId { get; set; }
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        //public StateEnum StateId { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<CustomerFacility> CustomerFacilities { get; set; }
        public virtual ICollection<CustomerDevice> CustomerDevices { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Zone> Zones { get; set; }
        public virtual ICollection<InventoryZone> InventoryZones { get; set; }
        public virtual ICollection<Bin> Bins { get; set; }
        public virtual ICollection<InventoryBin> InventoryBins { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<Receive> Receives { get; set; }
        public virtual ICollection<PutAway> PutAways { get; set; }
        public virtual ICollection<PutAwayBin> PutAwayBins { get; set; }
        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
        public virtual ICollection<TransferCurrent> TransferCurrents { get; set; }
        public virtual ICollection<TransferNew> TransferNews { get; set; }
        public virtual ICollection<AdjustBinQuantity> AdjustBinQuantities { get; set; }
        public virtual ICollection<TransferZoneBin> TransferZoneBins { get; set; }
        public virtual ICollection<SaleOrder> SaleOrders { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<OrderLineBin> OrderLineBins { get; set; }
        public virtual ICollection<Recall> Recalls { get; set; }
        public virtual ICollection<RecallBin> RecallBins { get; set; }
        public virtual ICollection<Lot> Lots { get; set; }
        public virtual ICollection<Pallet> Pallets { get; set; }
        public virtual ICollection<PalletInventory> PalletInventories { get; set; }
    }
}