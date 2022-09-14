using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class PutAwayBinConfiguration : IEntityTypeConfiguration<PutAwayBin>
    {
        public void Configure(EntityTypeBuilder<PutAwayBin> builder)
        {
            builder.ToTable("PutAways_Bins");

            builder.HasKey(x => x.PutAwayBinId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.ReceivedDateTime)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.PutAwayBins)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PutAwaysBins_CustomerLocations");

            builder.HasOne(x => x.PutAway)
               .WithMany(x => x.PutAwayBins)
               .HasForeignKey(x => x.PutAwayId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PutAwaysBins_PutAways");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.PutAwayBins)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PutAwaysBins_Bins");
        }
    }
}
