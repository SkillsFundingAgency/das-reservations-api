using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.UnitOfWork.NServiceBus;

namespace SFA.DAS.Reservations.Api.StartupExtensions
{
    public static class NServiceBusStartUp
    {
        public static void StartNServiceBus(this UpdateableServiceProvider serviceProvider,
            IConfiguration configuration, bool configurationIsLocalOrDev)
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Reservations.Api")
                
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox()
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => new SqlConnection(configuration["Reservations:ConnectionString"]))
                .UseUnitOfWork();

            if (configurationIsLocalOrDev)
            {
                endpointConfiguration.UseLearningTransport();
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(
                    configuration["Reservations:NServiceBusConnectionString"], r => { });
            }

            if (!string.IsNullOrEmpty(configuration["Reservations:NServiceBusLicense"]))
            {
                endpointConfiguration.License(configuration["Reservations:NServiceBusLicense"]);
            }
            

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();
        }
    }
}
