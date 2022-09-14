using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.ToTable("Vendors");
                
            builder.HasKey(x => x.VendorId);

            builder.Property(x => x.VendorNo)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Address1)
                .IsRequired()
                .HasMaxLength(550);

            builder.Property(x => x.Address2)
                .HasMaxLength(550);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.StateProvince)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.ZipPostalCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PointOfContact)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.Vendors)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Vendors_Customers");
        }
    }
}
