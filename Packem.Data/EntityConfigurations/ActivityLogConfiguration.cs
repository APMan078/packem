using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(x => x.ActivityLogId);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.ActivityDateTime)
                .IsRequired();

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.OldQty)
                .IsRequired();

            builder.Property(x => x.NewQty)
                .IsRequired();

            //builder.Property(x => x.Notes)
            //    .HasMaxLength(550);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.ActivityLogs)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ActivityLogs_Customers");

            builder.HasOne(x => x.Inventory)
               .WithMany(x => x.ActivityLogs)
               .HasForeignKey(x => x.InventoryId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ActivityLogs_Inventories");

            builder.HasOne(x => x.User)
               .WithMany(x => x.ActivityLogs)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ActivityLogs_Users");

            builder.HasOne(x => x.Zone)
               .WithMany(x => x.ActivityLogs)
               .HasForeignKey(x => x.ZoneId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ActivityLogs_Zones");

            builder.HasOne(x => x.Bin)
               .WithMany(x => x.ActivityLogs)
               .HasForeignKey(x => x.BinId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_ActivityLogs_Bins");
        }
    }
}