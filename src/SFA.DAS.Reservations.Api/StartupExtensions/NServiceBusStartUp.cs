using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

namespace SFA.DAS.Reservations.Api.StartupExtensions
{
    public static class NServiceBusStartUp
    {
        private const string EndPointName = "SFA.DAS.Reservations.Api";
        private const string AzureResource = "https://database.windows.net/";

        public static void StartNServiceBus(this UpdateableServiceProvider serviceProvider,
            IConfiguration configuration, bool configurationIsLocalOrDev)
        {
            var endpointConfiguration = new EndpointConfiguration(EndPointName)
                .UseInstallers()
                .UseErrorQueue($"{EndPointName}-errors")
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox(true)
                .UseServicesBuilder(serviceProvider)                
                .UseSqlServerPersistence(() => GetConnectionString(configurationIsLocalOrDev, configuration["Reservations:ConnectionString"]))
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

        private static DbConnection GetConnectionString(bool configurationIsLocalOrDev, string connectionString)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            return configurationIsLocalOrDev
                ? new SqlConnection(connectionString)
                : new SqlConnection
                {
                    ConnectionString = connectionString,
                    AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                };
        }
    }
}
