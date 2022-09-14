using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("PurchaseOrders");

            builder.HasKey(x => x.PurchaseOrderId);

            builder.Property(x => x.PurchaseOrderNo)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.ShipVia)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.OrderQty)
                .IsRequired();

            builder.Property(x => x.Remaining)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.PurchaseOrders)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PurchaseOrders_CustomerLocations");

            builder.HasOne(x => x.CustomerFacility)
               .WithMany(x => x.PurchaseOrders)
               .HasForeignKey(x => x.CustomerFacilityId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PurchaseOrders_CustomerFacilities");

            builder.HasOne(x => x.Vendor)
               .WithMany(x => x.PurchaseOrders)
               .HasForeignKey(x => x.VendorId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PurchaseOrders_Vendors");
        }
    }
}
