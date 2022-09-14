using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class CustomerFacilityConfiguration : IEntityTypeConfiguration<CustomerFacility>
    {
        public void Configure(EntityTypeBuilder<CustomerFacility> builder)
        {
            builder.ToTable("CustomerFacilities");

            builder.HasKey(x => x.CustomerFacilityId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.CustomerFacilities)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_CustomerFacilities_CustomerLocations");
        }
    }
}
