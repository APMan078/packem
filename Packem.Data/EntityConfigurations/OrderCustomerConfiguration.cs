using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class OrderCustomerConfiguration : IEntityTypeConfiguration<OrderCustomer>
    {
        public void Configure(EntityTypeBuilder<OrderCustomer> builder)
        {
            builder.ToTable("OrderCustomers");

            builder.HasKey(x => x.OrderCustomerId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.PaymentType)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.OrderCustomers)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_OrderCustomers_Customers");
        }
    }
}