using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Integrations.Entities;

namespace Packem.Integrations.EntityConfigurations
{

    public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
        {
            builder.ToTable("po_line");

            builder.HasKey(x => x.po_no);

        }
    }
}
