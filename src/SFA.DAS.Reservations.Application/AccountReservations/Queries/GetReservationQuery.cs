using System;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetReservationQuery : IRequest<GetReservationResponse>
    {
        public Guid Id { get; set; }
    }
}