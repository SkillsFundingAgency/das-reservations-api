using MediatR;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy
{
    public class BulkCreateReservationsWithNonLevyCommandHandler : IRequestHandler<BulkCreateReservationsWithNonLevyCommand, BulkCreateReservationsWithNonLevyResult>
    {
        private readonly IMediator _mediator;

        public BulkCreateReservationsWithNonLevyCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BulkCreateReservationsWithNonLevyResult> Handle(BulkCreateReservationsWithNonLevyCommand request, CancellationToken cancellationToken)
        {
            var result = new BulkCreateReservationsWithNonLevyResult();
            var levyAccounts = request.Reservations.Where(x => x.IsLevyAccount).ToList();
            var nonLevyAccounts = request.Reservations.Where(x => !x.IsLevyAccount).ToList();

            result.BulkCreateResults.AddRange(await CreateReservationsForLevyAccounts(levyAccounts));
            result.BulkCreateResults.AddRange(await CreateReservationForNonLevyAccounts(nonLevyAccounts));

            return result;
        }

        private async Task<List<BulkCreateReservationResult>> CreateReservationForNonLevyAccounts(List<BulkCreateReservations> nonLevyAccounts)
        {
            List<BulkCreateReservationResult> results = new List<BulkCreateReservationResult>();
            foreach (var nonLevyEntity in nonLevyAccounts)
            {
                var createdReservation = await _mediator.Send(new CreateAccountReservationCommand
                {
                    AccountId = nonLevyEntity.AccountId,
                    AccountLegalEntityId = nonLevyEntity.AccountLegalEntityId,
                    AccountLegalEntityName = nonLevyEntity.AccountLegalEntityName,
                    CourseId = nonLevyEntity.CourseId,
                    CreatedDate = nonLevyEntity.CreatedDate,
                    Id = nonLevyEntity.Id,
                    IsLevyAccount = nonLevyEntity.IsLevyAccount,
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

        private async Task<List<BulkCreateReservationResult>> CreateReservationsForLevyAccounts(List<BulkCreateReservations> levyAccounts)
        {
            List<BulkCreateReservationResult> results = new List<BulkCreateReservationResult>();
            var levyGroupedByAccountLegalEntities = levyAccounts.GroupBy(x => new { x.AccountLegalEntityId, x.TransferSenderAccountId });
            foreach (var levyEntity in levyGroupedByAccountLegalEntities)
            {
                var createdReservations = await _mediator.Send(new BulkCreateAccountReservationsCommand { AccountLegalEntityId = levyEntity.Key.AccountLegalEntityId, TransferSenderAccountId = levyEntity.Key.TransferSenderAccountId, ReservationCount = (uint)levyEntity.Count() });

                var mergedReservationIds = createdReservations.ReservationIds.Zip(levyEntity.Select(x => x.ULN), (reservationId, uln) => new BulkCreateReservationResult { ReservationId = reservationId, ULN = uln });
                results.AddRange(mergedReservationIds);
            }

            return results;
        }
    }
}
