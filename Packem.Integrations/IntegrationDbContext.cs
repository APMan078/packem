using Microsoft.EntityFrameworkCore;

using Packem.Integrations.Common.Interfaces;
using Packem.Integrations.Entities;
using Packem.Integrations.EntityConfigurations;
using Packem.Integrations.ExtensionMethods;

namespace Packem.Integrations
{
    public partial class IntegrationDbContext : DbContext
    {
        public virtual DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public virtual DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }

        public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PurchaseOrderHeaderConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderLineConfiguration());
            builder.ApplyConfiguration(new VendorConfiguration());

            // https://www.thereformedprogrammer.net/ef-core-in-depth-soft-deleting-data-with-global-query-filters/
            foreach (var x in builder.Model.GetEntityTypes())
            {
                //other automated configurations left out
                if (typeof(ISoftDelete).IsAssignableFrom(x.ClrType))
                {
                    x.AddSoftDeleteQueryFilter();
                }
            }
        }

        private void HandleDelete()
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted);
            foreach (var entity in entities)
            {
                if (entity.Entity is ISoftDelete)
                {
                    entity.State = EntityState.Modified;
                    var delete = entity.Entity as ISoftDelete;
                    delete.Deleted = true;
                }
            }
        }
    }
}