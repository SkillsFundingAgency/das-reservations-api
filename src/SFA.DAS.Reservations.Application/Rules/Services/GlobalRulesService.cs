using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class GlobalRulesService : IGlobalRulesService
    {
        private readonly IGlobalRuleRepository _repository;
        private readonly ReservationsConfiguration _options;

        public GlobalRulesService(IGlobalRuleRepository repository, IOptions<ReservationsConfiguration> options)
        {
            _repository = repository;
            _options = options.Value;
        }

        public async Task<IList<GlobalRule>> GetRules()
        {
            var result = await _repository.GetGlobalRules(DateTime.UtcNow);

            var globalRules = result.Select(globalRule => new GlobalRule(globalRule)).ToList();
            return globalRules;
        }

        public async Task<GlobalRule> CheckReservationAgainstRules(IReservationRequest request)
        {
            var resultsList = await _repository.GetGlobalRules(request.StartDate);

            if (resultsList == null || !resultsList.Any())
            {
                return null;
            }

            foreach (var result in resultsList)
            {
                var globalRule = new GlobalRule(result);
                var reservation = new Reservation(
                    request.Id,
                    request.AccountId,
                    request.StartDate,
                    _options.ExpiryPeriodInMonths,
                    request.AccountLegalEntityName,
                    request.CourseId,
                    request.ProviderId,
                    request.AccountLegalEntityId,
                    request.IsLevyAccount);

                switch (globalRule.Restriction)
                {
                    case AccountRestriction.All:
                        return globalRule;
                    case AccountRestriction.NonLevy when !reservation.IsLevyAccount:
                        return globalRule;
                    case AccountRestriction.Levy when reservation.IsLevyAccount:
                        return globalRule;
                }
            }

            return null;
        }
    }
}