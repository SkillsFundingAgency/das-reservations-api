using System;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class ValidateReservationQuery : IRequest<ValidateReservationResponse>
    {
        public Guid ReservationId { get; set; }
        public string CourseCode { get; set; }
        public DateTime StartDate { get; set; }
    }
}
