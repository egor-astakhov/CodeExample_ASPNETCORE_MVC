using Integral.Application.Common.Persistence;
using Integral.Application.Common.Security;
using Integral.Application.Common.Services;
using Integral.Infrastructure.Identity;
using Integral.Infrastructure.Persistence;
using Integral.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Integral.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            ConfigureAuthentication(services);

            AddDbContext(services, configuration);

            AddIdentity(services);

            AddAuthorization(services);

            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IApplicationSettingService, ApplicationSettingService>();
            services.AddTransient<IEmailServiceFactory, EmailServiceFactory>();
            services.AddTransient<IFileService, FileService>();

            return services;
        }

        private static void ConfigureAuthentication(IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("ApplicationDb"),
                    serverOptions =>
                    {
                        serverOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    });
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<IDeferredDbContext, DeferredDbContext>();
        }

        private static void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        private static void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(configure =>
            {
                configure.AddPolicy(Policy.OnlyForAdmins, policy => {
                    policy.RequireRole(Role.Admin);
                });
            });
        }
    }
}
