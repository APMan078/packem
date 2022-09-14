using Microsoft.EntityFrameworkCore;
using Packem.Integrations.Interfaces;
using Packem.Integrations.Services;

namespace Packem.Integrations
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<IntegrationDbContext>(opt =>
              opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Packem.Integrations")));
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(x => x.MapControllers());
        }
    }
}
