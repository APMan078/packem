using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class CustomerDeviceTokenConfiguration : IEntityTypeConfiguration<CustomerDeviceToken>
    {
        public void Configure(EntityTypeBuilder<CustomerDeviceToken> builder)
        {
            builder.ToTable("CustomerDeviceTokens");

            builder.HasKey(x => x.CustomerDeviceTokenId);

            builder.Property(x => x.DeviceToken)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.AddedDateTime)
                .IsRequired();

            builder.Property(x => x.IsValidated)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerDevice)
               .WithMany(x => x.CustomerDeviceTokens)
               .HasForeignKey(x => x.CustomerDeviceId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_CustomerDeviceTokens_CustomerDevices");
        }
    }
}
