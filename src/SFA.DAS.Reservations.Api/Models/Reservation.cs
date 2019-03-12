using System;

namespace SFA.DAS.Reservations.Api.Models
{
    public class Reservation
    {
        public long AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public string CourseId { get; set; }
    }
}
