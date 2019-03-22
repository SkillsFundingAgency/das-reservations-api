using System;

namespace SFA.DAS.Reservations.Api.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public string CourseId { get; set; }
        public int? ProviderId { get; set; }
        public long? LegalEntityAccountId { get; set; }
        public string AccountLegalEntityName { get; set; }
    }
}
