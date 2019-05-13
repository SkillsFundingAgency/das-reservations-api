using System;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class ValidateReservationQuery : IRequest<ValidateReservationResponse>
    {
        public Guid ReservationId { get; set; }
        public string CourseId { get; set; }
        public DateTime TrainingStartDate { get; set; }
    }
}
