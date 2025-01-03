﻿using System;
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
    public class GetAccountReservationStatusQueryHandler : IRequestHandler<GetAccountReservationStatusQuery, GetAccountReservationStatusResponse>
    {
        private readonly IValidator<GetAccountReservationStatusQuery> _validator;
        private readonly IGlobalRulesService _rulesService;
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;
        private readonly IAccountsService _accountsService;
        private readonly IAccountReservationService _accountReservationService;

        public GetAccountReservationStatusQueryHandler(
            IValidator<GetAccountReservationStatusQuery> validator,
            IGlobalRulesService rulesService,
            IAccountLegalEntitiesService accountLegalEntitiesService,
            IAccountsService accountsService,
            IAccountReservationService accountReservationService)
        {
            _validator = validator;
            _rulesService = rulesService;
            _accountLegalEntitiesService = accountLegalEntitiesService;
            _accountsService = accountsService;
            _accountReservationService = accountReservationService;
        }

        public async Task<GetAccountReservationStatusResponse> Handle(GetAccountReservationStatusQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            var accountId = request.TransferSenderAccountId ?? request.AccountId;

            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation", 
                    validationResult.ValidationDictionary
                        .Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var accountLegalEntities = await _accountLegalEntitiesService.GetAccountLegalEntities(accountId);

            var account = await _accountsService.GetAccount(accountId);

            if (accountLegalEntities == null || accountLegalEntities.Count == 0)
            {
                throw new EntityNotFoundException<Domain.Entities.AccountLegalEntity>();
            }

            var accountReservations = await _accountReservationService.GetAccountReservations(accountId);
            var numOfPendingReservations = accountReservations.Count(x => !x.IsExpired && x.Status == ReservationStatus.Pending);

            return new GetAccountReservationStatusResponse
            {
                CanAutoCreateReservations = account.IsLevy,
                HasReachedReservationsLimit = await _rulesService.HasReachedReservationLimit(accountId, account.IsLevy),
                HasPendingReservations = numOfPendingReservations > 0,
                AccountLegalEntityAgreementStatus = accountLegalEntities
                    .ToDictionary(key=>key.AccountLegalEntityId, value => value.AgreementSigned)
            };
        }
    }
}