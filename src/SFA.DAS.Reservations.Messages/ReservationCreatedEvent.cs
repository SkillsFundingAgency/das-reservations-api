using System;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Messages
{
    public class ReservationCreatedEvent
    {

        public ReservationCreatedEvent(Reservation reservation)
        {
            if (reservation == null)
            {
                return;
            }

            Id = reservation.Id;
            AccountLegalEntityId = reservation.AccountLegalEntityId;
            AccountLegalEntityName = reservation.AccountLegalEntityName;
            CourseId = reservation.Course?.CourseId;
            StartDate = reservation.StartDate;
            CourseName = reservation.Course?.Title;
            EndDate = reservation.ExpiryDate;
            CreatedDate = reservation.CreatedDate;
        }

        public Guid Id { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public string CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public string CourseName { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
