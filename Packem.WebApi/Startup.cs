using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Packem.Data;
using Packem.Data.Helpers;
using Packem.Domain.Common.Enums;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Entities;
using Packem.WebApi.Common.CustomProviders;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Packem.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Packem.WebApi")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                        ClockSkew = TimeSpan.Zero
                    });

            //services.AddAuthentication(SecurityTokenAuthOptions.DefaultScemeName)
            //    .AddScheme<SecurityTokenAuthOptions, ApplicationTokenAuthHandler>(SecurityTokenAuthOptions.DefaultScemeName,
            //        opts =>
            //        {
            //            // you can change the token header name here by :
            //            //     opts.TokenHeaderName = "X-Custom-Token-Header";
            //        });

            services.AddAuthentication(DeviceTokenAuthOptions.DeviceTokenScemeName)
                .AddScheme<DeviceTokenAuthOptions, DeviceTokenAuthHandler>(DeviceTokenAuthOptions.DeviceTokenScemeName,
                    opts =>
                    {
                        // you can change the token header name here by :
                        //     opts.TokenHeaderName = "X-Custom-Token-Header";
                    });

            services.AddData();

            services.AddRazorPages();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Pack'em API",
                    Version = "v1",
                    Description = "v1 API Documentation"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer Scheme. Example: Bearer <TOKEN>",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpLogging();

            //Enable swagger/middleware to serve Swagger as JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pack'em API v1");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = "Pack'em API Documentation";
                c.DocExpansion(DocExpansion.None);
            });

            var supportedCultures = new[] { "en-US" };
            var localicationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localicationOptions);
            app.UseSwagger();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default",
                pattern: "{controller=Home}/{action=Index}/{id}");
            });

            // create roles
            CreateRoles(app).Wait();
        }

        /// <summary>
        /// Creates the Admin and User Roles on startup if they do not exist, and assigns Admin User specified in config to Admin Role.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private async Task CreateRoles(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
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
                            Name = Configuration["AppSettings:AdminName"],
                            Username = Configuration["AppSettings:AdminUsername"],
                            Email = Configuration["AppSettings:AdminEmail"],
                            RoleId = RoleEnum.SuperAdmin.ToInt()
                        };
                        string adminUserPassword = Configuration["AppSettings:AdminPassword"];
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
    }
}
