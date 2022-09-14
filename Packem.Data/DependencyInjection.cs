using Microsoft.Extensions.DependencyInjection;
using Packem.Data.Interfaces;
using Packem.Data.Services;

namespace Packem.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services)
        {
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExceptionService, ExceptionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerLocationService, CustomerLocationService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<ICustomerFacilityService, CustomerFacilityService>();
            services.AddScoped<ICustomerDeviceService, CustomerDeviceService>();
            services.AddScoped<ICustomerDeviceTokenService, CustomerDeviceTokenService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IZoneService, ZoneService>();
            services.AddScoped<IBinService, BinService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<IReceiveService, ReceiveService>();
            services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
            services.AddScoped<IPutAwayService, PutAwayService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<IAdjustBinQuantityService, AdjustBinQuantityService>();
            services.AddScoped<ISaleOrderService, SaleOrderService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOrderLineService, OrderLineService>();
            services.AddScoped<IRecallService, RecallService>();
            services.AddScoped<IOrderCustomerService, OrderCustomerService>();
            services.AddScoped<IOrderCustomerAddressService, OrderCustomerAddressService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ILotService, LotService>();
            services.AddScoped<ILicensePlateService, LicensePlateService>();
            services.AddScoped<IPalletService, PalletService>();

            return services;
        }
    }
}