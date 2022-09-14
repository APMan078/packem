using Microsoft.EntityFrameworkCore;
using Packem.Data.EntityConfigurations;
using Packem.Data.ExtensionMethods;
using Packem.Domain.Common.Interfaces;
using Packem.Domain.Entities;
using System;
using System.Linq;

namespace Packem.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerLocation> CustomerLocations { get; set; }
        public virtual DbSet<CustomerFacility> CustomerFacilities { get; set; }
        public virtual DbSet<CustomerDevice> CustomerDevices { get; set; }
        public virtual DbSet<CustomerDeviceToken> CustomerDeviceTokens { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemVendor> ItemVendors { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Zone> Zones { get; set; }
        public virtual DbSet<InventoryZone> InventoryZones { get; set; }
        public virtual DbSet<Bin> Bins { get; set; }
        public virtual DbSet<InventoryBin> InventoryBins { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<Receive> Receives { get; set; }
        public virtual DbSet<PutAway> PutAways { get; set; }
        public virtual DbSet<PutAwayBin> PutAwayBins { get; set; }
        public virtual DbSet<Receipt> Receipts { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }
        public virtual DbSet<TransferCurrent> TransferCurrents { get; set; }
        public virtual DbSet<TransferNew> TransferNews { get; set; }
        public virtual DbSet<TransferZoneBin> TransferZoneBins { get; set; }
        public virtual DbSet<AdjustBinQuantity> AdjustBinQuantities { get; set; }
        public virtual DbSet<SaleOrder> SaleOrders { get; set; }
        public virtual DbSet<OrderLine> OrderLines { get; set; }
        public virtual DbSet<OrderLineBin> OrderLineBins { get; set; }
        public virtual DbSet<Recall> Recalls { get; set; }
        public virtual DbSet<RecallBin> RecallBins { get; set; }
        public virtual DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public virtual DbSet<UnitOfMeasureCustomer> UnitOfMeasureCustomers { get; set; }
        public virtual DbSet<OrderCustomer> OrderCustomers { get; set; }
        public virtual DbSet<OrderCustomerAddress> OrderCustomerAddresses { get; set; }
        public virtual DbSet<UserRoleVendor> UserRoleVendors { get; set; }
        public virtual DbSet<Lot> Lots { get; set; }
        public virtual DbSet<LicensePlate> LicensePlates { get; set; }
        public virtual DbSet<LicensePlateItem> LicensePlateItems { get; set; }
        public virtual DbSet<Pallet> Pallets { get; set; }
        public virtual DbSet<PalletInventory> PalletInventories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new CustomerLocationConfiguration());
            builder.ApplyConfiguration(new CustomerFacilityConfiguration());
            builder.ApplyConfiguration(new CustomerDeviceConfiguration());
            builder.ApplyConfiguration(new CustomerDeviceTokenConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new ErrorLogConfiguration());
            builder.ApplyConfiguration(new VendorConfiguration());
            builder.ApplyConfiguration(new ItemConfiguration());
            builder.ApplyConfiguration(new ItemVendorConfiguration());
            builder.ApplyConfiguration(new InventoryConfiguration());
            builder.ApplyConfiguration(new ZoneConfiguration());
            builder.ApplyConfiguration(new InventoryZoneConfiguration());
            builder.ApplyConfiguration(new BinConfiguration());
            builder.ApplyConfiguration(new InventoryBinConfiguration());
            builder.ApplyConfiguration(new ActivityLogConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderConfiguration());
            builder.ApplyConfiguration(new ReceiveConfiguration());
            builder.ApplyConfiguration(new PutAwayConfiguration());
            builder.ApplyConfiguration(new PutAwayBinConfiguration());
            builder.ApplyConfiguration(new ReceiptConfiguration());
            builder.ApplyConfiguration(new TransferConfiguration());
            builder.ApplyConfiguration(new TransferCurrentConfiguration());
            builder.ApplyConfiguration(new TransferNewConfiguration());
            builder.ApplyConfiguration(new TransferZoneBinConfiguration());
            builder.ApplyConfiguration(new AdjustBinQuantityConfiguration());
            builder.ApplyConfiguration(new SaleOrderConfiguration());
            builder.ApplyConfiguration(new OrderLineConfiguration());
            builder.ApplyConfiguration(new OrderLineBinConfiguration());
            builder.ApplyConfiguration(new RecallConfiguration());
            builder.ApplyConfiguration(new RecallBinConfiguration());
            builder.ApplyConfiguration(new UnitOfMeasureConfiguration());
            builder.ApplyConfiguration(new UnitOfMeasureCustomerConfiguration());
            builder.ApplyConfiguration(new OrderCustomerConfiguration());
            builder.ApplyConfiguration(new OrderCustomerAddressConfiguration());
            builder.ApplyConfiguration(new UserRoleVendorConfiguration());
            builder.ApplyConfiguration(new LotConfiguration());
            builder.ApplyConfiguration(new LicensePlateConfiguration());
            builder.ApplyConfiguration(new LicensePlateItemConfiguration());
            builder.ApplyConfiguration(new PalletConfiguration());
            builder.ApplyConfiguration(new PalletInventoryConfiguration());

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

        //public override int SaveChanges()
        //{
        //    HandleDelete();
        //    return base.SaveChanges();
        //}

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    HandleDelete();
        //    return base.SaveChangesAsync(cancellationToken);
        //}
    }
}