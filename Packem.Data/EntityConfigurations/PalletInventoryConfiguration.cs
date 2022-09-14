using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class PalletInventoryConfiguration : IEntityTypeConfiguration<PalletInventory>
    {
        public void Configure(EntityTypeBuilder<PalletInventory> builder)
        {
            builder.ToTable("Pallets_Inventories");

            builder.HasKey(x => x.PalletInventoryId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.PalletInventories)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PalletsInventories_Customers");

            builder.HasOne(x => x.CustomerFacility)
               .WithMany(x => x.PalletInventories)
               .HasForeignKey(x => x.CustomerFacilityId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PalletsInventories_CustomerFacilities");

            builder.HasOne(x => x.Pallet)
               .WithMany(x => x.PalletInventories)
               .HasForeignKey(x => x.PalletId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PalletsInventories_Pallets");

            builder.HasOne(x => x.Inventory)
               .WithMany(x => x.PalletInventories)
               .HasForeignKey(x => x.InventoryId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PalletsInventories_Inventories");

            builder.HasOne(x => x.LicensePlateItem)
               .WithOne(x => x.PalletInventory)
               .HasForeignKey<PalletInventory>(x => x.LicensePlateItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PalletsInventories_LicensePlatesItems");
        }
    }
}