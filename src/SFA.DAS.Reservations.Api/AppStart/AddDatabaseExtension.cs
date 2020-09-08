using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Courses.Api.AppStart
{
    public static class AddDatabaseExtension
    {
        public static void AddDatabaseRegistration(this IServiceCollection services, ReservationsConfiguration config, string environmentName)
        {
            if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.Courses"), ServiceLifetime.Transient);
            }
            else if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(config.ConnectionString), ServiceLifetime.Transient);
            }
            else
            {
                services.AddSingleton(new AzureServiceTokenProvider());
                services.AddDbContext<ReservationsDataContext>(ServiceLifetime.Transient);
            }

            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());
            services.AddTransient(provider => new Lazy<ReservationsDataContext>(provider.GetService<ReservationsDataContext>()));

        }
    }
}
