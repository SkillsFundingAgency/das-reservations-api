using System.Collections.Generic;

namespace SFA.DAS.Reservations.Api.Models
{
    public class ValidateReservationViewModel
    {
        public ICollection<ReservationValidationErrorViewModel> ValidationErrors { get; set; }
    }
}