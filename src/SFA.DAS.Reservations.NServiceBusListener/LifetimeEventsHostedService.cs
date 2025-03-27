using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.NServiceBusListener
{
    internal class LifetimeEventsHostedService(
        ILogger<LifetimeEventsHostedService> logger,
        IApplicationLifetime appLifetime,
        NServiceBusConsole console)
        : IHostedService
    {
        private readonly ILogger _logger = logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            await console.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await console.Stop();
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}
