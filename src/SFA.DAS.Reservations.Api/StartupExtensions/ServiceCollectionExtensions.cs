using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.UnitOfWork;
using SFA.DAS.UnitOfWork.NServiceBus;
using SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox;

namespace SFA.DAS.Reservations.Api.StartupExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBusClientUnitOfWork(this IServiceCollection services)
        {
            services.TryAddScoped<IEventPublisher, EventPublisher>();

            return services.AddUnitOfWork()
                .AddScoped<IUnitOfWork, UnitOfWork.NServiceBus.ClientOutbox.UnitOfWork>()
                .AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
        }
    }
}