using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class ValidateReservationResponse
    {
        public ICollection<ReservationValidationError> Errors { get; set; }
    }
}
