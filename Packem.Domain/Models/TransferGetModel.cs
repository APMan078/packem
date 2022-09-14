using Packem.Domain.Common.Enums;
using System;

namespace Packem.Domain.Models
{
    public class TransferGetModel
    {
        public int TransferId { get; set; }
        public int CustomerLocationId { get; set; }
        public int ItemId { get; set; }
        public int TransferCurrentId { get; set; }
        public int? TransferNewId { get; set; }
        public int Qty { get; set; }
        public int Remaining { get; set; }
        public TransferStatusEnum Status { get; set; }
        public DateTime TransferDateTime { get; set; }
        public DateTime? ReceivedDateTime { get; set; }
    }
}