﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class Reservation : IReservationRequest
    {
        public Reservation(Guid id, long accountId, DateTime startDate, int expiryPeriodInMonths, string accountLegalEntityName, string courseId = null, int? providerId = null, long? accountLegalEntityId = null, bool isLevyAccount = false)
        {
            Id = id;
            AccountId = accountId;
            StartDate = startDate;
            Status = ReservationStatus.Pending;
            CreatedDate = DateTime.UtcNow;
            ExpiryDate = GetExpiryDateFromStartDate(expiryPeriodInMonths);
            CourseId = courseId;
            ProviderId = providerId;
            AccountLegalEntityId = accountLegalEntityId;
            AccountLegalEntityName = accountLegalEntityName;
            IsLevyAccount = isLevyAccount;
        }

        public Reservation(Func<DateTime, Task<IList<Rule>>> rules,
            Guid id,
            long accountId,
            bool isLevyAccount,
            DateTime createdDate,
            DateTime startDate,
            DateTime expiryDate,
            ReservationStatus status,
            Course reservationCourse,
            int? providerId,
            long? accountLegalEntityId, string accountLegalEntityName)
        {
            Id = id;
            AccountId = accountId;
            IsLevyAccount = isLevyAccount;
            CreatedDate = createdDate;
            StartDate = startDate;
            ExpiryDate = expiryDate;
            Status = status;
            Rules = rules != null ? GetRulesForAccountType(GetRules(rules)) : null;
            Course = MapCourse(reservationCourse);
            ProviderId = providerId;
            AccountLegalEntityId = accountLegalEntityId;
            AccountLegalEntityName = accountLegalEntityName;
        }

        public Guid Id { get; }

        public long AccountId { get; }

        public bool IsLevyAccount { get; }

        public DateTime CreatedDate { get; }

        public DateTime StartDate { get; }

        public DateTime ExpiryDate { get; }

        public bool IsActive => ExpiryDate >= DateTime.UtcNow;

        [JsonIgnore]
        public string CourseId { get; }

        public ApprenticeshipCourse.Course Course { get; }

        public ICollection<ReservationRule> Rules { get; }

        public ReservationStatus Status { get; }
        public int? ProviderId { get; }
        public long? AccountLegalEntityId { get; }
        public string AccountLegalEntityName { get; }

        private IList<Rule> GetRules(Func<DateTime, Task<IList<Rule>>> getRules)
        {
            var task = getRules(StartDate);
            return task.Result;
        }

        private ICollection<ReservationRule> GetRulesForAccountType(IList<Rule> rules)
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

        private static ApprenticeshipCourse.Course MapCourse(Course reservationCourse)
        {
            return reservationCourse == null ? null : new ApprenticeshipCourse.Course(reservationCourse);
        }
		
		private DateTime GetExpiryDateFromStartDate(int expiryPeriodInMonths)
        {
            var expiryDate = StartDate.AddMonths(expiryPeriodInMonths);
            var lastDayInMonth = DateTime.DaysInMonth(expiryDate.Year, expiryDate.Month);
            return new DateTime(expiryDate.Year, expiryDate.Month, lastDayInMonth);
        }
    }
}