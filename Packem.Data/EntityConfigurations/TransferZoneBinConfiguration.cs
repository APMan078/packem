using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class TransferZoneBinConfiguration : IEntityTypeConfiguration<TransferZoneBin>
    {
        public void Configure(EntityTypeBuilder<TransferZoneBin> builder)
        {
            builder.ToTable("Transfers_Zones_Bins");

            builder.HasKey(x => x.TransferZoneBinId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.ReceivedDateTime)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.TransferZoneBins)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferZoneBins_CustomerLocations");

            builder.HasOne(x => x.Transfer)
               .WithMany(x => x.TransferZoneBins)
               .HasForeignKey(x => x.TransferId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferZoneBins_Transfers");

            builder.HasOne(x => x.Zone)
               .WithMany(x => x.TransferZoneBins)
               .HasForeignKey(x => x.ZoneId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferZoneBins_Zones");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.TransferZoneBins)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferZoneBins_Bins");
        }
    }
}