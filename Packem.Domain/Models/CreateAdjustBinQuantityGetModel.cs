namespace Packem.Domain.Models
{
    public class CreateAdjustBinQuantityGetModel
    {
        public int CustomerId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ItemId { get; set; }
        public int BinId { get; set; }
        public string Message { get; set; }
    }
}