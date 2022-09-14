using System;

namespace Packem.Domain.Models
{
    public class TransferLookupDeviceGetModel
    {
        public int TransferId { get; set; }
        public int ItemId { get; set; }
        public string ItemSKU { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUOM { get; set; }
        public string CurrentZone { get; set; }
        public int CurrentZoneId { get; set; }
        public string CurrentBin { get; set; }
        public int CurrentBinId { get; set; }
        public int CurrentBinQty { get; set; }
        public string NewZone { get; set; }
        public int? NewZoneId { get; set; }
        public string NewBin { get; set; }
        public int? NewBinId { get; set; }
        public int QtyToTransfer { get; set; }
        public int Remaining { get; set; }
        public string LotNo { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}