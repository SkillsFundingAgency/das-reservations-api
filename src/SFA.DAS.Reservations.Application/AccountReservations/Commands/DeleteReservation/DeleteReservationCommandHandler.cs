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

            await _reservationService.DeleteReservation(command.ReservationId);
            
            _context.AddEvent(new ReservationDeletedEvent(command.ReservationId));

            return Unit.Value;
        }
    }
}