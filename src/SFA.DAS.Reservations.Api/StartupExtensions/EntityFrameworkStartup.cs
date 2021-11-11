using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NServiceBus.Persistence;
using SFA.DAS.NServiceBus.SqlServer.Data;
using SFA.DAS.Reservations.Api.AppStart;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.UnitOfWork.Context;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Api.StartupExtensions
{
    public static class EntityFrameworkStartup
    {
        public static IServiceCollection AddEntityFramework(this IServiceCollection services, bool configurationIsLocalOrDev, IOptions<ReservationsConfiguration> config)
        {
            return services.AddScoped(p =>
            {
                var unitOfWorkContext = p.GetService<IUnitOfWorkContext>();
                ReservationsDataContext dbContext;
                try
                {                    
                    var synchronizedStorageSession = unitOfWorkContext.Get<SynchronizedStorageSession>();
                    var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
                    var optionsBuilder = new DbContextOptionsBuilder<ReservationsDataContext>().UseSqlServer(sqlStorageSession.Connection);
                    dbContext = new ReservationsDataContext(optionsBuilder.Options);
                    dbContext.Database.UseTransaction(sqlStorageSession.Transaction);
                }
                catch (KeyNotFoundException)
                {
                    var connection = AddDatabaseExtension.GetConnectionString(configurationIsLocalOrDev, config.Value.ConnectionString);
                    var optionsBuilder = new DbContextOptionsBuilder<ReservationsDataContext>().UseSqlServer(connection);
                    dbContext = new ReservationsDataContext(optionsBuilder.Options);
                }

                return dbContext;
            });
        }
    }
}