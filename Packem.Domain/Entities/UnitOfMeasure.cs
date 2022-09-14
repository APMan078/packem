using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System.Collections.Generic;
namespace Packem.Domain.Entities
{
    public partial class UnitOfMeasure : ISoftDelete
    {
        public UnitOfMeasure()
        {
            UnitOfMeasureCustomers = new HashSet<UnitOfMeasureCustomer>();
            Items = new HashSet<Item>();
        }
        public int UnitOfMeasureId { get; set; }
        public int? CustomerId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public UnitOfMeasureTypeEnum Type { get; set; }
        public bool Deleted { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<UnitOfMeasureCustomer> UnitOfMeasureCustomers { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}