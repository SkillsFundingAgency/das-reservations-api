using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;

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
