using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.NServiceBusListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        public static async Task Run()
        {
            Console.WriteLine("Setting up IOC...");

            var host = new HostBuilder()
                .UseEnvironment("local")
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("host.json");
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ReservationsConfiguration>(hostContext.Configuration.GetSection("Reservations"));
                    services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsConfiguration>>().Value);
                    services.AddTransient<NServiceBusListener.NServiceBusConsole>();
                    services.AddHostedService<LifetimeEventsHostedService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            var nServiceBusConsole = new NServiceBusConsole();

            Console.WriteLine("Running host...");

            await host.RunAsync();
        }
    }
}


