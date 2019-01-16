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
        public string CourseId { get; set; }
        public AccountRestriction Restriction { get; set; }
        public Course Course { get; set; }
    }
}
