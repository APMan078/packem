using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class InventoryBinConfiguration : IEntityTypeConfiguration<InventoryBin>
    {
        public void Configure(EntityTypeBuilder<InventoryBin> builder)
        {
            builder.ToTable("Inventories_Bins");

            builder.HasKey(x => x.InventoryBinId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.InventoryBins)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_InventoriesBins_CustomerLocations");

            builder.HasOne(x => x.Inventory)
               .WithMany(x => x.InventoryBins)
               .HasForeignKey(x => x.InventoryId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_InventoriesBins_Inventories");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.InventoryBins)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_InventoriesBins_Bins");

            builder.HasOne(x => x.Lot)
               .WithMany(x => x.InventoryBins)
               .HasForeignKey(x => x.LotId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_InventoriesBins_Lots");
        }
    }
}