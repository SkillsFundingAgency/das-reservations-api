﻿using System;
using System.Linq;
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
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            Reservation reservation;

            if (request.CourseId != null)
            {
                reservation = await _accountReservationService.CreateAccountReservation(request.Id, request.AccountId, request.StartDate, request.CourseId);
            }
            else
            {
                reservation = await _accountReservationService.CreateAccountReservation(request.Id, request.AccountId, request.StartDate);
            }

            return new CreateAccountReservationResult
            {
                Reservation = reservation
            };
        }
    }
}
