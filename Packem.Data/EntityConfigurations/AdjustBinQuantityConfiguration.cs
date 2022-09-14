using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class AdjustBinQuantityConfiguration : IEntityTypeConfiguration<AdjustBinQuantity>
    {
        public void Configure(EntityTypeBuilder<AdjustBinQuantity> builder)
        {
            builder.ToTable("AdjustBinQuantities");

            builder.HasKey(x => x.AdjustBinQuantityId);

            builder.Property(x => x.OldQty)
                .IsRequired();

            builder.Property(x => x.NewQty)
                .IsRequired();

            builder.Property(x => x.AdjustDateTime)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.AdjustBinQuantities)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_AdjustBinQuantities_CustomerLocations");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.AdjustBinQuantities)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_AdjustBinQuantities_Items");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.AdjustBinQuantities)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_AdjustBinQuantities_Bins");
        }
    }
}