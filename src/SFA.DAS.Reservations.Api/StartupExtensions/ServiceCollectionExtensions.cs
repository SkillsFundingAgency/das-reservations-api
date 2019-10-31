using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nest;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Services;
using SFA.DAS.UnitOfWork.Pipeline;

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
            var connectionPool = new  SingleNodeConnectionPool(new Uri(configuration.ElasticSearchServerUrl));

            var settings = new ConnectionSettings(connectionPool);

            collection.AddTransient<IElasticClient>(sp => new ElasticClient(settings));
        }
    }
}