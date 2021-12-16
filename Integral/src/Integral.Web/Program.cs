using System;
using System.Threading.Tasks;
using Integral.Application.Common.Services;
using Integral.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Integral.Web
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting Integral.Web application...");
                await Init(args);
            }
            catch (Exception exception)
            {
                logger.Fatal(exception, "Stopped because of exception.");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        private static async Task Init(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();

            CreateDirectories(scope.ServiceProvider);

            await MigrateDatabase(scope.ServiceProvider);

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(builder =>
                 {
                     builder.ClearProviders();
                 })
                .UseNLog();
        }

        public static void CreateDirectories(IServiceProvider serviceProvider)
        {
            serviceProvider
                .GetRequiredService<IFileService>()
                .CreateDefaultDirectories();
        }

        private static async Task MigrateDatabase(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            await ApplicationDbContextSeed.SeedAsync(serviceProvider);
        }
    }
}
