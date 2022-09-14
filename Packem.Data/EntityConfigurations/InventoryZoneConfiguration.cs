using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class InventoryZoneConfiguration : IEntityTypeConfiguration<InventoryZone>
    {
        public void Configure(EntityTypeBuilder<InventoryZone> builder)
        {
            builder.ToTable("Inventories_Zones");

            builder.HasKey(x => x.InventoryZoneId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.InventoryZones)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_InventoryZones_CustomerLocations");

            builder.HasOne(x => x.Inventory)
               .WithMany(x => x.InventoryZones)
               .HasForeignKey(x => x.InventoryId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_InventoryZones_Inventories");

            builder.HasOne(x => x.Zone)
               .WithMany(x => x.InventoryZones)
               .HasForeignKey(x => x.ZoneId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_InventoryZones_Areas");
        }
    }
}
