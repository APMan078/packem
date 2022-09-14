namespace Packem.Domain.Models
{
    public class TransferRequestCreateModel
    {
        public int? UserId { get; set; }
        public int? TransferId { get; set; }
        public BinGetCreateModel NewBinGetCreate { get; set; }
        public int? QtyTransfered { get; set; }
    }
}