using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class PutAwayConfiguration : IEntityTypeConfiguration<PutAway>
    {
        public void Configure(EntityTypeBuilder<PutAway> builder)
        {
            builder.ToTable("PutAways");

            builder.HasKey(x => x.PutAwayId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Remaining)
                .IsRequired();

            builder.Property(x => x.PutAwayType)
                .IsRequired();

            builder.Property(x => x.PutAwayDate)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.PutAways)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PutAways_CustomerLocations");

            builder.HasOne(x => x.Receive)
               .WithMany(x => x.PutAways)
               .HasForeignKey(x => x.ReceiveId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PutAways_Receives");

            builder.HasOne(x => x.Receipt)
               .WithMany(x => x.PutAways)
               .HasForeignKey(x => x.ReceiptId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PutAways_Receipts");

            builder.HasOne(x => x.LicensePlate)
               .WithOne(x => x.PutAway)
               .HasForeignKey<PutAway>(x => x.LicensePlateId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PutAways_LicensePlates");
        }
    }
}