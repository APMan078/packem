using Packem.Domain.Common.Enums;

namespace Packem.Domain.Models
{
    public class RecallStatusUpdateModel
    {
        public int? RecallId { get; set; }
        public RecallStatusEnum? Status { get; set; }
    }
}