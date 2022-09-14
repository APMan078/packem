using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class PalletConfiguration : IEntityTypeConfiguration<Pallet>
    {
        public void Configure(EntityTypeBuilder<Pallet> builder)
        {
            builder.ToTable("Pallets");

            builder.HasKey(x => x.PalletId);

            builder.Property(x => x.CreatedDateTime)
                .IsRequired();

            builder.Property(x => x.MixedPallet)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Pallets)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Pallets_CustomerLocations");

            builder.HasOne(x => x.CustomerFacility)
               .WithMany(x => x.Pallets)
               .HasForeignKey(x => x.CustomerFacilityId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Pallets_CustomerFacilities");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.Pallets)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Pallets_Bins");
        }
    }
}