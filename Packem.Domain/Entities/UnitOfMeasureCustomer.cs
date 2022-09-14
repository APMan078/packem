using Packem.Domain.Common.Interfaces;

namespace Packem.Domain.Entities
{
    public partial class UnitOfMeasureCustomer : ISoftDelete
    {
        public int UnitOfMeasureCustomerId { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public int? CustomerId { get; set; }
        public int? Order { get; set; }
        public bool Deleted { get; set; }

        public virtual UnitOfMeasure UnitOfMeasure { get; set; }
        public virtual Customer Customer { get; set; }
    }
}