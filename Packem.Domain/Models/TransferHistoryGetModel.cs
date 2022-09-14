using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class TransferHistoryGetModel
    {
        public class ToTransferLocation
        {
            public int? ToZoneId { get; set; }
            public string ToZone { get; set; }
            public int? ToBinId { get; set; }
            public string ToBin { get; set; }
        }

        public class Transfer
        {
            public int TransferId { get; set; }
            public string Status { get; set; }
            public int ItemId { get; set; }
            public string ItemSKU { get; set; }
            public string ItemDescription { get; set; }
            public string ItemUOM { get; set; }
            public int FromZoneId { get; set; }
            public string FromZone { get; set; }
            public int FromBinId { get; set; }
            public string FromBin { get; set; }
            public int? ToZoneId { get; set; }
            public string ToZone { get; set; }
            public int? ToBinId { get; set; }
            public string ToBin { get; set; }
            public int QtyTransfer { get; set; }

            public Transfer()
            {
                ToTransferLocations = new List<ToTransferLocation>();
            }

            public IEnumerable<ToTransferLocation> ToTransferLocations { get; set; }
        }

        public TransferHistoryGetModel()
        {
            Transfers = new List<Transfer>();
        }

        public int Pending { get; set; }
        public int Completed { get; set; }
        public int UnitToday { get; set; }
        public IEnumerable<Transfer> Transfers { get; set; }
    }
}