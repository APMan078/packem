using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
    {
        public void Configure(EntityTypeBuilder<Receipt> builder)
        {
            builder.ToTable("Receipts");

            builder.HasKey(x => x.ReceiptId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Remaining)
                .IsRequired();

            builder.Property(x => x.ReceiptDate)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Receipts)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Receipts_CustomerLocations");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.Receipts)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Receipts_Items");

            builder.HasOne(x => x.Lot)
               .WithMany(x => x.Receipts)
               .HasForeignKey(x => x.LotId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Receipts_Lots");
        }
    }
}