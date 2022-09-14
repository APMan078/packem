using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.CustomerId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(550);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PointOfContact)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.ContactEmail)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();
        }
    }
}