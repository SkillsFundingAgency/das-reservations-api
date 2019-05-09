using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;

namespace SFA.DAS.Reservations.NServiceBusListener
{
    public class NServiceBusConsole
    {
        private IEndpointInstance _endpointInstance;

        public async Task Start()
        {
            Console.WriteLine("Opening NServiceBus Endpoint...");
            
            var configuration = new EndpointConfiguration("SFA.DAS.Reservations.Api");

            if (!string.IsNullOrEmpty(EnvironmentVariables.NServiceBusLicense))
            {
                configuration.License(EnvironmentVariables.NServiceBusLicense);
            }

            var serialization = configuration.UseSerialization<NewtonsoftSerializer>();
            serialization.WriterCreator(s => new JsonTextWriter(new StreamWriter(s, new UTF8Encoding(false))));
            serialization.ReaderCreator(s => new JsonTextReader(new StreamReader(s, new UTF8Encoding(false))));

            var transport = configuration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(EnvironmentVariables.NServiceBusConnectionString);
            transport.Transactions(TransportTransactionMode.ReceiveOnly);

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
