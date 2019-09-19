using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;

namespace SFA.DAS.Reservations.NServiceBusListener
{
    public class NServiceBusConsole
    {
        private IEndpointInstance _endpointInstance;

        public async Task Start()
        {
            Console.WriteLine("Opening NServiceBus Endpoint...");

            var configuration = new EndpointConfiguration("SFA.Reservations.NServiceBusListener")
                .UseAzureServiceBusTransport(EnvironmentVariables.NServiceBusConnectionString, r => { })
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();
            

            _endpointInstance = await Endpoint.Start(configuration)
                .ConfigureAwait(false);

            Console.WriteLine("Endpoint started and awaiting messages...");
        }


        public async Task Stop()
        {
            await _endpointInstance.Stop()
                .ConfigureAwait(false);

            Console.WriteLine("Endpoint closed...");
        }

    }
}
