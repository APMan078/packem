using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NUnit.Framework;
using Packem.Data;
using Packem.Data.Helpers;
using Packem.Domain.Common.Enums;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Entities;
using Packem.WebApi;
using Respawn;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.IntegrationTests
{
    [SetUpFixture]
    public class Testing
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static IConfiguration _configuration = null!;
        private static IServiceScopeFactory _scopeFactory = null!;
        private static Checkpoint _checkpoint = null!;
        private static string? _currentUserId;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            _factory = new CustomWebApplicationFactory();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            _configuration = _factory.Services.GetRequiredService<IConfiguration>();

            _checkpoint = new Checkpoint
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            };

            //EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            context.Database.Migrate();
        }

        public static T GetService<T>()
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }

        public static async Task ResetState()
        {
            //var context = GetService<ApplicationDbContext>();
            //await context.Database.EnsureCreatedAsync();

            //var migrator = GetService<IMigrator>();
            //await migrator.MigrateAsync();

            //throw new Exception(_configuration.GetConnectionString("IntegrationTestConnection"));

            using (var conn = new NpgsqlConnection(_configuration.GetConnectionString("IntegrationTestConnection")))
            {
                await conn.OpenAsync();

                await _checkpoint.Reset(conn);
            }

            _currentUserId = null;
        }

        public static async Task InitData()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var _context = services.GetService<ApplicationDbContext>())
                {
                    //initializing custom roles 
                    var roles = new List<string>();
                    foreach (RoleEnum r in Enum.GetValues(typeof(RoleEnum)))
                    {
                        roles.Add(r.ToString());
                    }

                    foreach (var x in roles)
                    {
                        var roleExist = await _context.Roles.AnyAsync(r => r.Name.Trim().ToLower() == x.Trim().ToLower());
                        if (!roleExist)
                        {
                            //create the roles if they do not exist
                            var entity = new Role
                            {
                                Name = x
                            };

                            _context.Add(entity);
                        }
                    }

                    await _context.SaveChangesAsync();

                    try
                    {
                        //create Admin User with email and username specified in config file (appsettings.json)
                        var adminUser = new User
                        {
                            //Ensure you have these values in your appsettings.json file, in the AppSettings section (not created by default)
                            // A bug in Microsoft's Identity code requires that the Username is the Email
                            Name = _configuration["AppSettings:AdminName"],
                            Username = _configuration["AppSettings:AdminUsername"],
                            Email = _configuration["AppSettings:AdminEmail"],
                            RoleId = RoleEnum.SuperAdmin.ToInt()
                        };
                        string adminUserPassword = _configuration["AppSettings:AdminPassword"];
                        // Check if the Admin User has already been created
                        var exists = await _context.Users.AnyAsync(x => x.Username.Trim().ToLower() == adminUser.Username.Trim().ToLower());

                        // Create Admin User if they do not exist
                        if (!exists)
                        {
                            var pass = CryptographicHelper.EncryptPassword(adminUserPassword);

                            var entity = new User
                            {
                                Name = adminUser.Name,
                                Username = adminUser.Username,
                                Email = adminUser.Email,
                                RoleId = adminUser.RoleId,
                                Password = pass.Hash,
                                PasswordSalt = pass.Salt,
                                IsActive = true,
                            };

                            _context.Add(entity);
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        throw new ArgumentNullException("Failed to create Admin User: make sure Admin credentials are defined in appsettings.json.", ex);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
        }
    }
}
