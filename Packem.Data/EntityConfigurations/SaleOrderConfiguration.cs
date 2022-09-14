using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class SaleOrderConfiguration : IEntityTypeConfiguration<SaleOrder>
    {
        public void Configure(EntityTypeBuilder<SaleOrder> builder)
        {
            builder.ToTable("SaleOrders");

            builder.HasKey(x => x.SaleOrderId);

            builder.Property(x => x.SaleOrderNo)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.OrderQty)
                .IsRequired();

            builder.Property(x => x.Remaining)
                .IsRequired();

            builder.Property(x => x.PickingStatus)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.SaleOrders)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_SaleOrders_CustomerLocations");

            builder.HasOne(x => x.CustomerFacility)
               .WithMany(x => x.SaleOrders)
               .HasForeignKey(x => x.CustomerFacilityId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_SaleOrders_CustomerFacilities");

            builder.HasOne(x => x.OrderCustomer)
               .WithMany(x => x.SaleOrders)
               .HasForeignKey(x => x.OrderCustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_SaleOrders_OrderCustomers");

            builder.HasOne(x => x.ShippingAddress)
               .WithMany(x => x.SaleOrderShippingAddresses)
               .HasForeignKey(x => x.ShippingAddressId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_SaleOrders_ShippingAddresses");

            builder.HasOne(x => x.BillingAddress)
               .WithMany(x => x.SaleOrderBillingAddresses)
               .HasForeignKey(x => x.BillingAddressId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_SaleOrders_BillingAddresses");
        }
    }
}