using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class ItemVendorConfiguration : IEntityTypeConfiguration<ItemVendor>
    {
        public void Configure(EntityTypeBuilder<ItemVendor> builder)
        {
            builder.ToTable("Items_Vendors");

            builder.HasKey(x => x.ItemVendorId);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.ItemVendors)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ItemsVendors_Customers");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.ItemVendors)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ItemsVendors_Items");

            builder.HasOne(x => x.Vendor)
               .WithMany(x => x.ItemVendors)
               .HasForeignKey(x => x.VendorId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ItemsVendors_Vendors");
        }
    }
}
