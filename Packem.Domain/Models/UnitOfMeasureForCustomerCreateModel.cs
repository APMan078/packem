using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class UnitOfMeasureForCustomerCreateModel
    {
        public UnitOfMeasureForCustomerCreateModel()
        {
            UnitOfMeasureIds = new List<int>();
        }

        public int? CustomerId { get; set; }
        public IEnumerable<int> UnitOfMeasureIds { get; set; }
    }
}