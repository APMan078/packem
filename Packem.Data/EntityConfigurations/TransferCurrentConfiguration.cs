using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class TransferCurrentConfiguration : IEntityTypeConfiguration<TransferCurrent>
    {
        public void Configure(EntityTypeBuilder<TransferCurrent> builder)
        {
            builder.ToTable("TransferCurrents");

            builder.HasKey(x => x.TransferCurrentId);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.TransferCurrents)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferCurrents_CustomerLocations");

            builder.HasOne(x => x.CurrentZone)
               .WithMany(x => x.TransferCurrents)
               .HasForeignKey(x => x.CurrentZoneId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferCurrents_CurrentZones");

            builder.HasOne(x => x.CurrentBin)
               .WithMany(x => x.TransferCurrents)
               .HasForeignKey(x => x.CurrentBinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferCurrents_CurrentBins");
        }
    }
}