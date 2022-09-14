using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class OrderLineBinConfiguration : IEntityTypeConfiguration<OrderLineBin>
    {
        public void Configure(EntityTypeBuilder<OrderLineBin> builder)
        {
            builder.ToTable("OrderLines_Bins");

            builder.HasKey(x => x.OrderLineBinId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.PickDateTime)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.OrderLineBins)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_OrderLinesBins_CustomerLocations");

            builder.HasOne(x => x.OrderLine)
               .WithMany(x => x.OrderLineBins)
               .HasForeignKey(x => x.OrderLineId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_OrderLinesBins_OrderLines");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.OrderLineBins)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_OrderLinesBins_Bins");
        }
    }
}