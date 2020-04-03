using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Application.Account.Services;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Application.Courses.Services;
using SFA.DAS.Reservations.Application.ProviderPermissions.Services;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Infrastructure.AzureServiceBus;
using SFA.DAS.Reservations.Infrastructure.Configuration;

namespace SFA.DAS.Reservations.Api.AppStart
{
    public static class AddServiceExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services, IOptions<ReservationsConfiguration> config)
        {
            services.AddTransient<IUserRuleAcknowledgementRepository, UserRuleAcknowledgementRepository>();
            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<IRuleRepository, RuleRepository>();
            services.AddTransient<IGlobalRuleRepository, GlobalRuleRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<IAccountLegalEntitiesRepository, AccountLegalEntityRepository>();
            services.AddTransient<IAccountReservationService, AccountReservationService>();
            services.AddTransient<IAccountsService, AccountsService>();
            services.AddTransient<IRulesService, RulesService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IGlobalRulesService, GlobalRulesService>();
            services.AddTransient<IAvailableDatesService, AvailableDatesService>();
            services.AddTransient<IAccountLegalEntitiesService, AccountLegalEntitiesService>();
            services.AddTransient<IUserRuleAcknowledgementService, UserRuleAcknowledgementService>();
            services.AddTransient<IAzureQueueService, AzureQueueService>();
            services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
            services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
            services.AddTransient<IProviderPermissionService, ProviderPermissionsService>();

            services.AddSingleton<ICurrentDateTime>(config.Value.CurrentDateTime.HasValue
                ? new CurrentDateTime(config.Value.CurrentDateTime)
                : new CurrentDateTime());
        }
    }
}