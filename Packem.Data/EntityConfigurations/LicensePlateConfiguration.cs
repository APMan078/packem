using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class LicensePlateConfiguration : IEntityTypeConfiguration<LicensePlate>
    {
        public void Configure(EntityTypeBuilder<LicensePlate> builder)
        {
            builder.ToTable("LicensePlates");

            builder.HasKey(x => x.LicensePlateId);

            builder.Property(x => x.LicensePlateNo)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.LicensePlateType)
                .IsRequired();

            builder.Property(x => x.Printed)
                .IsRequired();

            builder.Property(x => x.CreatedDateTime)
                .IsRequired();

            builder.Property(x => x.Palletized)
               .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.LicensePlates)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_LicensePlates_Customers");

            builder.HasOne(x => x.Pallet)
               .WithMany(x => x.LicensePlates)
               .HasForeignKey(x => x.PalletId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_LicensePlates_Pallets");
        }
    }
}