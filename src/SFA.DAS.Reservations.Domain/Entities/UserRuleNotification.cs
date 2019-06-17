using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class UserRuleNotification
    {
        public long Id { get; set; }
        public long? CourseRuleId { get; set; }
        public long? GlobalRuleId { get; set; }
        public int? UkPrn { get; set; }
        public Guid? UserId { get; set; }
        public virtual Rule CourseRule { get; set; }
        public virtual GlobalRule GlobalRule { get; set; }
    }
}
