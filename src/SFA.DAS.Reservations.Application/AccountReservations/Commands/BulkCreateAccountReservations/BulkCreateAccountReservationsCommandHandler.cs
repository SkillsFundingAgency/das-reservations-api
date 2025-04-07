using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations
{
    public class BulkCreateAccountReservationsCommandHandler(
        IAccountReservationService accountReservationService,
        IValidator<BulkCreateAccountReservationsCommand> validator,
        IAccountLegalEntitiesService accountLegalEntitiesService)
        : IRequestHandler<BulkCreateAccountReservationsCommand, BulkCreateAccountReservationsResult>
    {
        public async Task<BulkCreateAccountReservationsResult> Handle(BulkCreateAccountReservationsCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation",
                    validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var accountLegalEntity = await accountLegalEntitiesService.GetAccountLegalEntity(command.AccountLegalEntityId);

            var reservationIds = await accountReservationService.BulkCreateAccountReservation(command.ReservationCount, command.AccountLegalEntityId, accountLegalEntity.AccountId, accountLegalEntity.AccountLegalEntityName, command.TransferSenderAccountId);

            return new BulkCreateAccountReservationsResult
            {
                ReservationIds = reservationIds
            };
        }
    }
}
