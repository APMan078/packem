using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class UnitOfMeasureCustomerConfiguration : IEntityTypeConfiguration<UnitOfMeasureCustomer>
    {
        public void Configure(EntityTypeBuilder<UnitOfMeasureCustomer> builder)
        {
            builder.ToTable("UnitOfMeasures_Customers");

            builder.HasKey(x => x.UnitOfMeasureCustomerId);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.UnitOfMeasure)
               .WithMany(x => x.UnitOfMeasureCustomers)
               .HasForeignKey(x => x.UnitOfMeasureId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_UnitOfMeasureCustomers_UnitOfMeasures");

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.UnitOfMeasureCustomers)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_UnitOfMeasureCustomers_Customers");
        }
    }
}