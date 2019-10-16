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
    public class DeleteReservationCommandHandler : IRequestHandler<DeleteReservationCommand>
    {
        private readonly IValidator<DeleteReservationCommand> _validator;
        private readonly IAccountReservationService _reservationService;
        private readonly IUnitOfWorkContext _context;

        public DeleteReservationCommandHandler(
            IValidator<DeleteReservationCommand> validator,
            IAccountReservationService reservationService,
            IUnitOfWorkContext context)
        {
            _validator = validator;
            _reservationService = reservationService;
            _context = context;
        }

        public async Task<Unit> Handle(DeleteReservationCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation",
                    validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var reservationToDelete = await _reservationService.GetReservation(command.ReservationId);

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
                reservationToDelete.ProviderId);

            await _reservationService.DeleteReservation(command.ReservationId);
            
            _context.AddEvent(deletedEvent);

            return Unit.Value;
        }
    }
}