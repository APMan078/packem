using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class LicensePlateItemConfiguration : IEntityTypeConfiguration<LicensePlateItem>
    {
        public void Configure(EntityTypeBuilder<LicensePlateItem> builder)
        {
            builder.ToTable("LicensePlates_Items");

            builder.HasKey(x => x.LicensePlateItemId);

            builder.Property(x => x.ReferenceNo)
                .HasMaxLength(250);

            builder.Property(x => x.TotalQty)
                .IsRequired();

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.LicensePlateItems)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_LicensePlatesItems_Customers");

            builder.HasOne(x => x.LicensePlate)
               .WithMany(x => x.LicensePlateItems)
               .HasForeignKey(x => x.LicensePlateId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_LicensePlatesItems_LicensePlates");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.LicensePlateItems)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_LicensePlatesItems_Items");

            builder.HasOne(x => x.Vendor)
               .WithMany(x => x.LicensePlateItems)
               .HasForeignKey(x => x.VendorId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_LicensePlatesItems_Vendors");

            builder.HasOne(x => x.Lot)
               .WithMany(x => x.LicensePlateItems)
               .HasForeignKey(x => x.LotId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_LicensePlatesItems_Lots");
        }
    }
}