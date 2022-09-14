using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");
            builder.HasKey(x => x.ItemId);
            //builder.Property(x => x.ItemNo)
            //    .IsRequired()
            //    .HasMaxLength(250);
            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(550);
            //builder.Property(x => x.UOM)
            //    .IsRequired()
            //    .HasMaxLength(250);
            builder.Property(x => x.Barcode)
                .HasMaxLength(55);
            builder.Property(x => x.SKU)
                .IsRequired()
                .HasMaxLength(55);
            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.Items)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Items_Customers");

            builder.HasOne(x => x.UnitOfMeasure)
               .WithMany(x => x.Items)
               .HasForeignKey(x => x.UnitOfMeasureId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Items_UnitOfMeasures");
        }
    }
}