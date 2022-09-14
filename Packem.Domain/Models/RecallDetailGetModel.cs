namespace Packem.Domain.Models
{
    public class RecallDetailGetModel
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int BinId { get; set; }
        public string BinName { get; set; }
        public int Qty { get; set; }
        public int Received { get; set; }
        public int Remaining { get; set; }
    }
}