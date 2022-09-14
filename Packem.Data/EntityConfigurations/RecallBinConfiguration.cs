using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class RecallBinConfiguration : IEntityTypeConfiguration<RecallBin>
    {
        public void Configure(EntityTypeBuilder<RecallBin> builder)
        {
            builder.ToTable("Recalls_Bins");

            builder.HasKey(x => x.RecallBinId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.PickDateTime)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.RecallBins)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_RecallsBins_CustomerLocations");

            builder.HasOne(x => x.Recall)
               .WithMany(x => x.RecallBins)
               .HasForeignKey(x => x.RecallId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_RecallsBins_Recalls");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.RecallBins)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_RecallsBins_Bins");
        }
    }
}