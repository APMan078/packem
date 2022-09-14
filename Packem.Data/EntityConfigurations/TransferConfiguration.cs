using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
        {
            builder.ToTable("Transfers");

            builder.HasKey(x => x.TransferId);

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.Remaining)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.TransferDateTime)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.CustomerLocation)
               .WithMany(x => x.Transfers)
               .HasForeignKey(x => x.CustomerLocationId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Transfers_CustomerLocations");

            builder.HasOne(x => x.Item)
               .WithMany(x => x.Transfers)
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Transfers_Items");

            builder.HasOne(x => x.TransferCurrent)
               .WithMany(x => x.Transfers)
               .HasForeignKey(x => x.TransferCurrentId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Transfers_TransferCurrents");

            builder.HasOne(x => x.TransferNew)
               .WithMany(x => x.Transfers)
               .HasForeignKey(x => x.TransferNewId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Transfers_TransferNews");
        }
    }
}