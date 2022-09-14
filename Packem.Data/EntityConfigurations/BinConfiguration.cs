using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class BinConfiguration : IEntityTypeConfiguration<Bin>
    {
        public void Configure(EntityTypeBuilder<Bin> builder)
        {
            builder.ToTable("Bins");

            builder.HasKey(x => x.BinId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.Property(x => x.Category)
                 .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Bins)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Bins_CustomerLocations");

            builder.HasOne(x => x.Zone)
               .WithMany(x => x.Bins)
               .HasForeignKey(x => x.ZoneId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Bins_Zones");
        }
    }
}
