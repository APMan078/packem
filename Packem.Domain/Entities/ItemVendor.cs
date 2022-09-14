using Packem.Domain.Common.Interfaces;

namespace Packem.Domain.Entities
{
    public partial class ItemVendor : ISoftDelete
    {
        public int ItemVendorId { get; set; }
        public int? CustomerId { get; set; }
        public int? ItemId { get; set; }
        public int? VendorId { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Item Item { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
