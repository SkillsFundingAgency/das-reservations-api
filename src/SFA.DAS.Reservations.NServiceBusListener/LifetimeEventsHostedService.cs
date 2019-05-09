using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.NServiceBusListener
{
    internal class LifetimeEventsHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly NServiceBusListener.NServiceBusConsole _console;

        public LifetimeEventsHostedService(
            ILogger<LifetimeEventsHostedService> logger, 
            IApplicationLifetime appLifetime,
            NServiceBusListener.NServiceBusConsole console)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _console = console;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            await _console.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _console.Stop();
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
