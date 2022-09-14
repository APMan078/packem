using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class RecallConfiguration : IEntityTypeConfiguration<Recall>
    {
        public void Configure(EntityTypeBuilder<Recall> builder)
        {
            builder.ToTable("Recalls");

            builder.HasKey(x => x.RecallId);

            builder.Property(x => x.RecallDate)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Recalls)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Recalls_CustomerLocations");

            builder.HasOne(x => x.CustomerFacility)
               .WithMany(x => x.Recalls)
               .HasForeignKey(x => x.CustomerFacilityId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Recalls_CustomerFacilities");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.Recalls)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Recalls_Items");
        }
    }
}