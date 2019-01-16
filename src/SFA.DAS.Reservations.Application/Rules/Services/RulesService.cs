﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class RulesService : IRulesService
    {
        private readonly IRuleRepository _ruleRepository;

        public RulesService(IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }
        public async Task<IList<ReservationRule>> GetRules()
        {
            var repositoryRules = await _ruleRepository.GetReservationRules(DateTime.UtcNow, null);

            var rules = repositoryRules.Select(repositoryRule => new ReservationRule
                {
                    Id = repositoryRule.Id,
                    Restriction = (AccountRestriction) repositoryRule.Restriction,
                    CreatedDate = repositoryRule.CreatedDate,
                    ActiveFrom = repositoryRule.ActiveFrom,
                    ActiveTo = repositoryRule.ActiveTo,
                    CourseId = repositoryRule.CourseId,
                    Course = new Course
                    {
                        CourseId = repositoryRule.Course.CourseId,
                        Level = repositoryRule.Course.Level.ToString(),
                        Title = repositoryRule.Course.Title
                    }
                }).ToList();

            return rules;
        }
    }
}
