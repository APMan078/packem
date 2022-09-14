namespace Packem.Domain.Models
{
    public class ReceiveLookupPOReceiveDeviceGetModel
    {
        public int ReceiveId { get; set; }
        public int ItemId { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int Remaining { get; set; }
    }
}
