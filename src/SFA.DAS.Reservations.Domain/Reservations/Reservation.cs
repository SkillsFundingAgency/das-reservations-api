using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class Reservation
    {
        public Reservation(long accountId, DateTime startDate, int expiryPeriodInMonths)
        {
            AccountId = accountId;
            StartDate = startDate;
            Status = ReservationStatus.Pending;
            CreatedDate = DateTime.UtcNow;
            ExpiryDate = startDate.AddMonths(expiryPeriodInMonths);
        }

        public Reservation(Func<DateTime, Task<IList<Rule>>> rules, 
            Guid id, 
            long accountId, 
            bool isLevyAccount, 
            DateTime createdDate, 
            DateTime startDate, 
            DateTime expiryDate, 
            ReservationStatus status)
        {
            Id = id;
            AccountId = accountId;
            IsLevyAccount = isLevyAccount;
            CreatedDate = createdDate;
            StartDate = startDate;
            ExpiryDate = expiryDate;
            Status = status;
            Rules = rules != null ? GetRulesForAccountType(GetRules(rules)) : null;
        }

        public Guid Id { get; }

        public long AccountId { get; }

        public bool IsLevyAccount { get; }

        public DateTime CreatedDate { get; }

        public DateTime StartDate { get; }

        public DateTime ExpiryDate { get; }

        public bool IsActive => ExpiryDate >= DateTime.UtcNow;

        public List<ReservationRule> Rules { get; }

        public ReservationStatus Status { get; }

        private IList<Rule> GetRules(Func<DateTime, Task<IList<Rule>>> getRules)
        {
            var task = getRules(StartDate);
            return task.Result;
        }

        private List<ReservationRule> GetRulesForAccountType(IList<Rule> rules)
        {
            return rules.Where(FilterByAccountType()).Select(x => new ReservationRule(x)).ToList();
        }

        private Func<Rule, bool> FilterByAccountType()
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