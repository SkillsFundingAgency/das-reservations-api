using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Application.Courses.Services;
using SFA.DAS.Reservations.Application.ProviderPermissions.Queries;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Infrastructure.Configuration;

namespace SFA.DAS.Reservations.Api.AppStart
{
    public static class AddMediatRExtension
    {
        public static void AddMediatRValidators(this IServiceCollection services)
        {
            services.AddScoped(typeof(IValidator<GetAccountReservationsQuery>),
                typeof(GetAccountReservationsValidator));
            services.AddScoped(typeof(IValidator<CreateAccountReservationCommand>),
                typeof(CreateAccountReservationValidator));
            services.AddScoped(typeof(IValidator<CreateUserRuleAcknowledgementCommand>),
                typeof(CreateUserRuleAcknowledgementCommandValidator));
            services.AddScoped(typeof(IValidator<GetReservationQuery>), typeof(GetReservationValidator));

            services.AddScoped(typeof(IValidator<GetAccountRulesQuery>), typeof(GetAccountRulesValidator));
            services.AddScoped(typeof(IValidator<GetAccountLegalEntitiesQuery>), typeof(GetAccountLegalEntitiesQueryValidator));
            services.AddScoped(typeof(IValidator<GetAccountLegalEntityQuery>), typeof(GetAccountLegalEntityQueryValidator));
            services.AddScoped(typeof(IValidator<GetAccountReservationStatusQuery>), typeof(GetAccountReservationStatusQueryValidator));
            services.AddScoped(typeof(IValidator<GetAvailableDatesQuery>), typeof(GetAvailableDatesValidator));
            services.AddScoped(typeof(IValidator<ValidateReservationQuery>), typeof(ValidateReservationValidator));
            services.AddScoped(typeof(IValidator<GetAvailableDatesQuery>), typeof(GetAvailableDatesValidator));
            services.AddScoped(typeof(IValidator<BulkCreateAccountReservationsCommand>), typeof(BulkCreateAccountReservationsCommandValidator));
            services.AddScoped(typeof(IValidator<DeleteReservationCommand>), typeof(DeleteReservationCommandValidator));
            services.AddScoped(typeof(IValidator<FindAccountReservationsQuery>), typeof(FindAccountReservationsValidator));
            services.AddScoped(typeof(IValidator<GetAccountLegalEntitiesForProviderQuery>), typeof(GetAccountLegalEntitiesForProviderValidator));
        }
    }

}
