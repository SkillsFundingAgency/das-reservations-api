using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Rule
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public long ApprenticeshipId { get; set; }
        public byte Restriction { get; set; }
        public virtual Apprenticeship ApprenticeshipCourse { get; set; }
    }
}
