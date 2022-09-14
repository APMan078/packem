using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class CustomerDeviceConfiguration : IEntityTypeConfiguration<CustomerDevice>
    {
        public void Configure(EntityTypeBuilder<CustomerDevice> builder)
        {
            builder.ToTable("CustomerDevices");

            builder.HasKey(x => x.CustomerDeviceId);

            builder.Property(x => x.SerialNumber)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.AddedDateTime)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.CustomerDevices)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_CustomerDevices_Customers");

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.CustomerDevices)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_CustomerDevices_CustomerLocations");
        }
    }
}
