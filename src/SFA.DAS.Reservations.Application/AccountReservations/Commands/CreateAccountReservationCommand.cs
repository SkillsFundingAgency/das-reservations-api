using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationCommand : IRequest<CreateAccountReservationResult>
    {
        public long AccountId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
