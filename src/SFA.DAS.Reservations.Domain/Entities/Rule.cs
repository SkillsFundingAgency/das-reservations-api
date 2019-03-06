using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Rule
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public byte Restriction { get; set; }
        public string CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
