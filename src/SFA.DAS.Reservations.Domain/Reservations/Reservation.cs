using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class Reservation
    {
        private readonly IRuleRepository _ruleRepository;

        public Reservation(IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public long Id { get; set; }
        public long AccountId { get; set; }
        public bool IsLevyAccount { get; set; }
        public long? ApprenticeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive => ExpiryDate <= DateTime.UtcNow;
        public List<ReservationRule> Rules { get; private set; }
        public long? VacancyId { get; set; }
        
        public virtual async Task GetReservationRules()
        {
            var rules = (await _ruleRepository.GetReservationRules(StartDate, ExpiryDate)).ToList();

            Rules = new List<ReservationRule>();

            foreach (var rule in rules.Where(FilterByAccountType()))
            {
                Rules.Add(new ReservationRule
                {
                    CreatedDate = rule.CreatedDate,
                    ActiveFrom = rule.ActiveFrom,
                    ActiveTo = rule.ActiveTo,
                    Id = rule.Id,
                    Restriction = (AccountRestriction) rule.Restriction,
                    ApprenticeshipId = rule.ApprenticeshipId,
                    ApprenticeshipCourse = new Apprenticeship
                    {
                        CourseId = rule.ApprenticeshipCourse.CourseId,
                        Id = rule.ApprenticeshipCourse.Id,
                        Level = rule.ApprenticeshipCourse.Level.ToString(),
                        Title = rule.ApprenticeshipCourse.Title
                    }
                });
            }
        }

        private Func<Entities.Rule, bool> FilterByAccountType()
        {
            var accountType = AccountRestriction.NonLevy;
            if (IsLevyAccount)
            {
                accountType = AccountRestriction.Levy;
            }

            return c => (c.Restriction == (byte) AccountRestriction.All || c.Restriction == (byte) accountType);

        }
    }
}