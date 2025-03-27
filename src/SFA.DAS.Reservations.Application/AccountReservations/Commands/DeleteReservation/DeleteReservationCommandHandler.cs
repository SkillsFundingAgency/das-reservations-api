using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation
{
    public class DeleteReservationCommandHandler(
        IValidator<DeleteReservationCommand> validator,
        IAccountReservationService reservationService,
        IUnitOfWorkContext context)
        : IRequestHandler<DeleteReservationCommand>
    {
        public async Task Handle(DeleteReservationCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation",
                    validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var reservationToDelete = await reservationService.GetReservation(command.ReservationId);

            var deletedEvent = new ReservationDeletedEvent(
                command.ReservationId,
                reservationToDelete.AccountId,
                reservationToDelete.AccountLegalEntityId,
                reservationToDelete.AccountLegalEntityName,
                reservationToDelete.StartDate.GetValueOrDefault(),
                reservationToDelete.ExpiryDate.GetValueOrDefault(),
                reservationToDelete.CreatedDate,
                reservationToDelete.Course?.CourseId,
                reservationToDelete.Course?.Title,
                reservationToDelete.Course?.Level,
                reservationToDelete.ProviderId,
                command.EmployerDeleted);

            await reservationService.DeleteReservation(command.ReservationId);
            
            context.AddEvent(deletedEvent);
        }
    }
}