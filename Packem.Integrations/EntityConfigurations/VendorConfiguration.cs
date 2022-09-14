using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Integrations.Entities;

namespace Packem.Integrations.EntityConfigurations
{

    public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.ToTable("vendor");

            builder.HasKey(x => x.vendor_id);

        }
    }
}
