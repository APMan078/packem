using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class CustomerLocationConfiguration : IEntityTypeConfiguration<CustomerLocation>
    {
        public void Configure(EntityTypeBuilder<CustomerLocation> builder)
        {
            builder.ToTable("CustomerLocations");

            builder.HasKey(x => x.CustomerLocationId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            //builder.Property(x => x.StateId)
            //    .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.Customer)
               .WithMany(x => x.CustomerLocations)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_CustomerLocations_Customers");
        }
    }
}
