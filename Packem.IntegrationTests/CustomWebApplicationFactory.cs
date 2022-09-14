using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Packem.Data;
using Packem.WebApi;
using System;

namespace Packem.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                var integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

                configurationBuilder.AddConfiguration(integrationConfig);
            });

            builder.ConfigureServices((builder, services) =>
            {
                //services
                //    .Remove<ICurrentUserService>()
                //    .AddTransient(provider => Mock.Of<ICurrentUserService>(s =>
                //        s.UserId == GetCurrentUserId()));

                services
                    .Remove<DbContextOptions<ApplicationDbContext>>()
                    .AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(opt =>
                        opt.UseNpgsql(builder.Configuration.GetConnectionString("IntegrationTestConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                ////services.AddDbContext<DatabaseContext>();
                //using (var context = new ApplicationDbContext())
                //{
                //    // NOTE: use Migrate(), if using migration.
                //    // else, EnsureCreated()
                //    //context.Database.EnsureCreated();
                //    context.Database.Migrate();
                //}

                //var context = Testing.GetService<ApplicationDbContext>();
                //context.Database.Migrate();
            });
        }
    }
}
