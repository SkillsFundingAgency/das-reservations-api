using System;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class ReservationRule
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public long ApprenticeshipId { get; set; }
        public AccountRestriction Restriction { get; set; }
        public Apprenticeship ApprenticeshipCourse { get; set; }
    }
}
