using Integral.Application.Storage.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Web.Services
{
    public class CleanupService : BackgroundService
    {
        private static readonly int DELAY = (int)TimeSpan.FromMinutes(20).TotalMilliseconds;

        private readonly ILogger<CleanupService> _logger;

        public CleanupService(IServiceProvider services, ILogger<CleanupService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = Services.CreateScope())
                {
                    try
                    {
                        await scope.ServiceProvider
                            .GetRequiredService<IMediator>()
                            .Send(new CleanupUnusedFilesCommand());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "The error occured during cleanup.");
                    }
                }

                await Task.Delay(DELAY, stoppingToken);
            }
        }
    }
}
