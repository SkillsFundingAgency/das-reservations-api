﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationCommandHandler : IRequestHandler<CreateAccountReservationCommand, CreateAccountReservationResult>
    {
        private readonly IAccountReservationService _accountReservationService;
        private readonly IValidator<CreateAccountReservationCommand> _validator;

        public CreateAccountReservationCommandHandler(IAccountReservationService accountReservationService, IValidator<CreateAccountReservationCommand> validator)
        {
            _accountReservationService = accountReservationService;
            _validator = validator;
        }

        public async Task<CreateAccountReservationResult> Handle(CreateAccountReservationCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new InvalidOperationException();
            }

            var reservationId = await _accountReservationService.CreateAccountReservation(request.AccountId, request.StartDate);

            return new CreateAccountReservationResult
            {
                ReservationId = reservationId
            };
        }
    }
}
