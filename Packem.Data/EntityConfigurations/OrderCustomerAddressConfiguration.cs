using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class OrderCustomerAddressConfiguration : IEntityTypeConfiguration<OrderCustomerAddress>
    {
        public void Configure(EntityTypeBuilder<OrderCustomerAddress> builder)
        {
            builder.ToTable("OrderCustomerAddresses");

            builder.HasKey(x => x.OrderCustomerAddressId);

            builder.Property(x => x.AddressType)
                .IsRequired();

            builder.Property(x => x.Address1)
                .IsRequired()
                .HasMaxLength(550);

            builder.Property(x => x.Address2)
                .HasMaxLength(550);

            builder.Property(x => x.StateProvince)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.ZipPostalCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(50);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.OrderCustomer)
               .WithMany(x => x.OrderCustomerAddresses)
               .HasForeignKey(x => x.OrderCustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_OrderCustomerAddresses_OrderCustomers");
        }
    }
}