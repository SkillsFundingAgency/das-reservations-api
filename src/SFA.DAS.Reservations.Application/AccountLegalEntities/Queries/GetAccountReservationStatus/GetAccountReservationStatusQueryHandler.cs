using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus
{
    public class GetAccountReservationStatusQueryHandler(
        IValidator<GetAccountReservationStatusQuery> validator,
        IGlobalRulesService rulesService,
        IAccountLegalEntitiesService accountLegalEntitiesService,
        IAccountsService accountsService,
        IAccountReservationService accountReservationService)
        : IRequestHandler<GetAccountReservationStatusQuery, GetAccountReservationStatusResponse>
    {
        public async Task<GetAccountReservationStatusResponse> Handle(GetAccountReservationStatusQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation", 
                    validationResult.ValidationDictionary
                        .Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var accountLegalEntities = await accountLegalEntitiesService.GetAccountLegalEntities(request.AccountId);

            var account = await accountsService.GetAccount(request.AccountId);

            if (accountLegalEntities == null || accountLegalEntities.Count == 0)
            {
                throw new EntityNotFoundException<Domain.Entities.AccountLegalEntity>();
            }

            var accountReservations = await accountReservationService.GetAccountReservations(request.AccountId);
            var numOfPendingReservations = accountReservations.Count(x => !x.IsExpired && x.Status == ReservationStatus.Pending);

            return new GetAccountReservationStatusResponse
            {
                CanAutoCreateReservations = account.IsLevy,
                HasReachedReservationsLimit = await rulesService.HasReachedReservationLimit(request.AccountId, account.IsLevy),
                HasPendingReservations = numOfPendingReservations > 0,
                AccountLegalEntityAgreementStatus = accountLegalEntities
                    .ToDictionary(key=>key.AccountLegalEntityId, value => value.AgreementSigned)
            };
        }
    }
}