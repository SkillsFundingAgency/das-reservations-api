using System;
using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Types;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Course
    {
        public string CourseId { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string ApprenticeshipType { get; set; }
        public LearningType? LearningType { get; set; }
        public virtual ICollection<Rule> ReservationRule { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
