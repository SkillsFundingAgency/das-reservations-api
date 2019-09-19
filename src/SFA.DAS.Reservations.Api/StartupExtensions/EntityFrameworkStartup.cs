using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Persistence;
using SFA.DAS.NServiceBus.SqlServer.Data;
using SFA.DAS.Reservations.Data;
using SFA.DAS.UnitOfWork;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.Reservations.Api.StartupExtensions
{
    public static class EntityFrameworkStartup
    {
        public static IServiceCollection AddEntityFramework(this IServiceCollection services)
        {
            return services.AddScoped(p =>
            {
                var unitOfWorkContext = p.GetService<IUnitOfWorkContext>();
                var synchronizedStorageSession = unitOfWorkContext.Get<SynchronizedStorageSession>();
                var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
                var optionsBuilder = new DbContextOptionsBuilder<ReservationsDataContext>().UseSqlServer(sqlStorageSession.Connection);
                var dbContext = new ReservationsDataContext(optionsBuilder.Options);

                dbContext.Database.UseTransaction(sqlStorageSession.Transaction);

                return dbContext;
            });
        }
    }
}