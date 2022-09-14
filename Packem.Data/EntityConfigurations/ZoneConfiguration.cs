using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zones");

            builder.HasKey(x => x.ZoneId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Zones)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Zones_CustomerLocations");

            builder.HasOne(x => x.CustomerFacility)
               .WithMany(x => x.Zones)
               .HasForeignKey(x => x.CustomerFacilityId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Zones_CustomerFacilities");
        }
    }
}
