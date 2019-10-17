using System;

namespace SFA.DAS.Reservations.Messages
{
    public class ReservationDeletedEvent
    {
        public ReservationDeletedEvent(Guid id, 
            long accountId, long accountLegalEntityId, string accountLegalEntityName,
            DateTime startDate, DateTime endDate, DateTime createdDate, 
            string courseId, string courseName, string courseLevel,
            uint? providerId)
        {
            Id = id;
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            AccountLegalEntityName = accountLegalEntityName;
            StartDate = startDate;
            EndDate = endDate;
            CreatedDate = createdDate;
            CourseId = courseId;
            CourseName = courseName;
            CourseLevel = courseLevel;
            ProviderId = providerId;
        }

        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseLevel { get; set; }
        public uint? ProviderId { get; set; }
    }
}
