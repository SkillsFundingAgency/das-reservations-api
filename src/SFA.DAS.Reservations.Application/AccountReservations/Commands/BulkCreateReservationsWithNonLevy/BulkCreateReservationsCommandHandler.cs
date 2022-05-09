using MediatR;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Application.BulkUpload.Queries;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy
{
    public class BulkCreateReservationsCommandHandler : IRequestHandler<BulkCreateReservationsCommand, BulkCreateReservationsWithNonLevyResult>
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<long, AccountLegalEntity> _cachedAccountLegalEntities;
        private IAccountLegalEntitiesService _accountLegalEntitiesService;

        public BulkCreateReservationsCommandHandler(IMediator mediator, IAccountLegalEntitiesService accountLegalEntitiesService)
        {
            _mediator = mediator;
            _cachedAccountLegalEntities = new Dictionary<long, AccountLegalEntity>();
            _accountLegalEntitiesService = accountLegalEntitiesService;
        }

        public async Task<BulkCreateReservationsWithNonLevyResult> Handle(BulkCreateReservationsCommand request, CancellationToken cancellationToken)
        {
            var result = new BulkCreateReservationsWithNonLevyResult();

            if (request.Reservations.All(
                    x => x.AccountLegalEntityId.HasValue 
                    && !string.IsNullOrWhiteSpace(x.CourseId) 
                    && x.ProviderId.HasValue && x.StartDate.HasValue))
            {
                var levyAccounts = await GetLevyAccounts(request.Reservations);
                var nonLevyAccounts = await GetNonLevyAccounts(request.Reservations);
                result.BulkCreateResults.AddRange(await CreateReservationsForLevyAccounts(levyAccounts));
                result.BulkCreateResults.AddRange(await CreateReservationForNonLevyAccounts(nonLevyAccounts));
            }

            return result;
        }

        private async Task<List<BulkCreateReservation>> GetLevyAccounts(List<BulkCreateReservation> reservations)
        {
            List<BulkCreateReservation> levyAccounts = new List<BulkCreateReservation>();
            foreach (var reservation in reservations)
            {
                var account = await  GetAccountLegalEntity(reservation.AccountLegalEntityId.Value);
                if (account.IsLevy || reservation.TransferSenderAccountId.HasValue)
                {
                    levyAccounts.Add(reservation);
                }
            }

            return levyAccounts;
        }

        private async Task<List<BulkCreateReservation>> GetNonLevyAccounts(List<BulkCreateReservation> reservations)
        {
            List<BulkCreateReservation> nonLevyAccounts = new List<BulkCreateReservation>();
            foreach (var reservation in reservations)
            {
                var account = await GetAccountLegalEntity(reservation.AccountLegalEntityId.Value);
                if (!account.IsLevy && !reservation.TransferSenderAccountId.HasValue)
                {
                    nonLevyAccounts.Add(reservation);
                }
            }

            return nonLevyAccounts;
        }

        private async Task<BulkValidationResults> Validate(BulkCreateReservationsCommand request)
        {
            var validateRequests = request.Reservations.Select(x => (BulkValidateRequest)x).ToList();
            var bulkValidationResults = await _mediator.Send(new BulkValidateCommand
            {
                Requests = validateRequests
            });
            return bulkValidationResults;
        }

        private async Task<List<BulkCreateReservationResult>> CreateReservationForNonLevyAccounts(List<BulkCreateReservation> nonLevyAccounts)
        {
            List<BulkCreateReservationResult> results = new List<BulkCreateReservationResult>();
            foreach (var nonLevyEntity in nonLevyAccounts)
            {
                var account = await GetAccountLegalEntity(nonLevyEntity.AccountLegalEntityId.Value);
                var createdReservation = await _mediator.Send(new CreateAccountReservationCommand
                {
                    AccountId = account.AccountId,
                    AccountLegalEntityId = nonLevyEntity.AccountLegalEntityId.Value,
                    AccountLegalEntityName = account.AccountLegalEntityName,
                    CourseId = nonLevyEntity.CourseId,
                    CreatedDate = DateTime.UtcNow,
                    Id = nonLevyEntity.Id,
                    IsLevyAccount = account.IsLevy,
                    ProviderId = nonLevyEntity.ProviderId,
                    StartDate = nonLevyEntity.StartDate,
                    TransferSenderAccountId = nonLevyEntity.TransferSenderAccountId,
                    UserId = nonLevyEntity.UserId
                });

                if (createdReservation.Rule != null || !createdReservation.AgreementSigned)
                {
                    throw new System.Exception($"Unable to create reservation for legal entity : {nonLevyEntity.AccountLegalEntityId} - Agreement signed {createdReservation.AgreementSigned} - Rule failed {createdReservation?.Rule?.RuleTypeText ?? "No rule fail"}");
                }

                results.Add(new BulkCreateReservationResult { ReservationId = createdReservation.Reservation.Id, ULN = nonLevyEntity.ULN });
            }
            return results;
        }

        private async Task<List<BulkCreateReservationResult>> CreateReservationsForLevyAccounts(List<BulkCreateReservation> levyAccounts)
        {
            List<BulkCreateReservationResult> results = new List<BulkCreateReservationResult>();
            var levyGroupedByAccountLegalEntities = levyAccounts.GroupBy(x => new { x.AccountLegalEntityId, x.TransferSenderAccountId });
            foreach (var levyEntity in levyGroupedByAccountLegalEntities)
            {
                var createdReservations = await _mediator.Send(new BulkCreateAccountReservationsCommand { AccountLegalEntityId = levyEntity.Key.AccountLegalEntityId.Value, TransferSenderAccountId = levyEntity.Key.TransferSenderAccountId, ReservationCount = (uint)levyEntity.Count() });

                var mergedReservationIds = createdReservations.ReservationIds.Zip(levyEntity.Select(x => x.ULN), (reservationId, uln) => new BulkCreateReservationResult { ReservationId = reservationId, ULN = uln });
                results.AddRange(mergedReservationIds);
            }

            return results;
        }

        private async Task<AccountLegalEntity> GetAccountLegalEntity(long accountLegalEntityId)
        {
            if (_cachedAccountLegalEntities.ContainsKey(accountLegalEntityId))
            {
                return _cachedAccountLegalEntities.GetValueOrDefault(accountLegalEntityId);
            }

            var accountLegalEntity = await _accountLegalEntitiesService.GetAccountLegalEntity(accountLegalEntityId);
            _cachedAccountLegalEntities.Add(accountLegalEntityId, accountLegalEntity);
            return accountLegalEntity;
        }
    }
}
