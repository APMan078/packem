using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class TransferNewConfiguration : IEntityTypeConfiguration<TransferNew>
    {
        public void Configure(EntityTypeBuilder<TransferNew> builder)
        {
            builder.ToTable("TransferNews");

            builder.HasKey(x => x.TransferNewId);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.TransferNews)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferNews_CustomerLocations");

            builder.HasOne(x => x.NewZone)
               .WithMany(x => x.TransferNews)
               .HasForeignKey(x => x.NewZoneId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferNews_NewZones");

            builder.HasOne(x => x.NewBin)
               .WithMany(x => x.TransferNews)
               .HasForeignKey(x => x.NewBinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TransferNews_NewBins");
        }
    }
}