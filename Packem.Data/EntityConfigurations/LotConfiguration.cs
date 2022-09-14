using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class LotConfiguration : IEntityTypeConfiguration<Lot>
    {
        public void Configure(EntityTypeBuilder<Lot> builder)
        {
            builder.ToTable("Lots");

            builder.HasKey(x => x.LotId);

            builder.Property(x => x.LotNo)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.ExpirationDate)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Lots)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Lots_CustomerLocations");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.Lots)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Lots_Items");
        }
    }
}