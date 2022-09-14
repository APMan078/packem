using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packem.Domain.Entities;

namespace Packem.Data.EntityConfigurations
{
    public class UserRoleVendorConfiguration : IEntityTypeConfiguration<UserRoleVendor>
    {
        public void Configure(EntityTypeBuilder<UserRoleVendor> builder)
        {
            builder.ToTable("Users_Roles_Vendors");

            builder.HasKey(x => x.UserRoleVendorId);

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.HasOne(x => x.User)
               .WithMany(x => x.UserRoleVendors)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_UsersRolesVendors_Users");

            builder.HasOne(x => x.Role)
               .WithMany(x => x.UserRoleVendors)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_UsersRolesVendors_Roles");

            builder.HasOne(x => x.Vendor)
               .WithMany(x => x.UserRoleVendors)
               .HasForeignKey(x => x.VendorId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_UsersRolesVendors_Vendors");
        }
    }
}