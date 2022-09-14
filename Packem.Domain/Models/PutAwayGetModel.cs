using System;

namespace Packem.Domain.Models
{
    public class PutAwayGetModel
    {
        public int PutAwayId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ReceiveId { get; set; }
        public int Qty { get; set; }
        public int Remaining { get; set; }
        public DateTime PutAwayDate { get; set; }
    }
}