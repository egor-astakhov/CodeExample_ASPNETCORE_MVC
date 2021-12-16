using FluentValidation.AspNetCore;
using Integral.Application;
using Integral.Application.Common.Configuration;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Services;
using Integral.Infrastructure;
using Integral.Web.Extensions;
using Integral.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace Integral.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);
            services.AddSingleton(factory => factory.GetRequiredService<IOptions<AppSettings>>().Value);

            services.AddApplication();

            services.AddInfrastructure(Configuration, Environment);

            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddHsts(options => {
                options.IncludeSubDomains = true;
                options.Preload = true;
                options.MaxAge = TimeSpan.FromMinutes(5);
            });

            services.AddControllersWithViews()
                .AddFluentValidation(options =>
                {
                    options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    options.ImplicitlyValidateChildProperties = true;
                    options.RegisterValidatorsFromAssemblyContaining<IApplicationDbContext>();
                    options.RegisterValidatorsFromAssemblyContaining<Startup>();
                });

            services.AddHostedService<CleanupService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSettings appSettings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(appSettings.Paths.ExternalWebRoot),
                RequestPath = new PathString("")
            });
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                endpoints.MapRobotsTxt(env);
            });
        }
    }
}
