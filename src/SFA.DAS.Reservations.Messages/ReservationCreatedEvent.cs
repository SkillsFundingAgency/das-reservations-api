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
            CourseId = reservation.Course.CourseId;
            StartDate = reservation.StartDate;
            CourseName = reservation.Course.Title;
            EndDate = reservation.ExpiryDate;
            CreatedDate = reservation.CreatedDate;
        }

        public Guid Id { get; }
        public long AccountLegalEntityId { get; }
        public string AccountLegalEntityName { get;  }
        public string CourseId { get; }
        public DateTime StartDate { get; }
        public string CourseName { get; }
        public DateTime EndDate { get;  }
        public DateTime CreatedDate { get;  }
    }
}
