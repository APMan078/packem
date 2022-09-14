using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class ReceiveConfiguration : IEntityTypeConfiguration<Receive>
    {
        public void Configure(EntityTypeBuilder<Receive> builder)
        {
            builder.ToTable("Receives");

            builder.HasKey(x => x.ReceiveId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Received)
                .IsRequired();

            builder.Property(x => x.Remaining)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Receives)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Receives_CustomerLocations");

            builder.HasOne(x => x.PurchaseOrder)
               .WithMany(x => x.Receives)
               .HasForeignKey(x => x.PurchaseOrderId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Receives_PurchaseOrders");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.Receives)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Receives_Items");

            builder.HasOne(x => x.Lot)
               .WithMany(x => x.Receives)
               .HasForeignKey(x => x.LotId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Receives_Lots");
        }
    }
}