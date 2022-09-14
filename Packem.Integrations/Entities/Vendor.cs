using System.ComponentModel.DataAnnotations.Schema;

namespace Packem.Integrations.Entities
{
    public partial class Vendor
    {
        public Vendor()
        {
            PurchaseOrderHeaders = new HashSet<PurchaseOrderHeader>();
        }
        [Column(TypeName = "decimal(19, 2)")]
        public decimal vendor_id { get; set; }
        public string VendorName { get; set; }


        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
    }
}
