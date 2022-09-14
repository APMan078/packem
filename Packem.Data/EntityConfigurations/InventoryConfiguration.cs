using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventories");

            builder.HasKey(x => x.InventoryId);

            builder.Property(x => x.QtyOnHand)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.Inventories)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Inventories_Customers");

            builder.HasOne(x => x.Item)
               .WithOne(x => x.Inventory)
               .HasForeignKey<Inventory>(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Inventories_Items");
        }
    }
}