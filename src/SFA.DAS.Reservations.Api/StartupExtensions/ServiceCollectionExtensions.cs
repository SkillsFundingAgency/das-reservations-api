using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Reservations.Data.AzureSearch;
using SFA.DAS.Reservations.Data.ElasticSearch;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Services;
using SFA.DAS.UnitOfWork.Pipeline;
using System;

namespace SFA.DAS.Reservations.Api.StartupExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBusClientUnitOfWork(this IServiceCollection services)
        {
            services.TryAddScoped<IEventPublisher, EventPublisher>();

            return services.AddUnitOfWork()
                .AddScoped<IUnitOfWork, UnitOfWork.NServiceBus.Features.ClientOutbox.Pipeline.UnitOfWork>()
                .AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
        }

        public static void AddElasticSearch(this IServiceCollection collection, ReservationsConfiguration configuration)
        {
            var connectionPool = new SingleNodeConnectionPool(new Uri(configuration.ElasticSearchServerUrl));

            var settings = new ConnectionConfiguration(connectionPool);

            if (!string.IsNullOrEmpty(configuration.ElasticSearchUsername) &&
                !string.IsNullOrEmpty(configuration.ElasticSearchPassword))
            {
                settings.BasicAuthentication(configuration.ElasticSearchUsername, configuration.ElasticSearchPassword);
            }
                        
            collection.AddTransient<IElasticLowLevelClient>(sp => new ElasticLowLevelClient(settings));
            collection.AddSingleton<IElasticSearchQueries, ElasticSearchQueries>();
        }

        public static IServiceCollection AddAzureSearch(this IServiceCollection collection, ReservationsConfiguration configuration)
        {
            collection.AddTransient<IAzureSearchHelper, AzureSearchHelper>();
            collection.AddTransient<IAzureSearchReservationIndexRepository, AzureSearchReservationIndexRepository>();
            return collection;
        }
    }
}
