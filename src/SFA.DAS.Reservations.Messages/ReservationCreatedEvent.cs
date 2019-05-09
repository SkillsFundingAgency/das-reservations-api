using System;

namespace SFA.DAS.Reservations.Messages
{
    public class ReservationCreatedEvent
    {
     
        public ReservationCreatedEvent(Guid id, long accountLegalEntityId, string accountLegalEntityName,
            string courseId, DateTime startDate, string courseName, DateTime endDate, DateTime createdDate)
        {
            Id = id;
            AccountLegalEntityId = accountLegalEntityId;
            AccountLegalEntityName = accountLegalEntityName;
            CourseId = courseId;
            StartDate = startDate;
            CourseName = courseName;
            EndDate = endDate;
            CreatedDate = createdDate;
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
