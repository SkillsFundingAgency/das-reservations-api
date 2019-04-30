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
        protected internal const int DefaultMaxNumberOfReservations = 3;
        private readonly IGlobalRuleRepository _repository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ReservationsConfiguration _options;

        public GlobalRulesService(IGlobalRuleRepository repository, IOptions<ReservationsConfiguration> options,
            IReservationRepository reservationRepository)
        {
            _repository = repository;
            _reservationRepository = reservationRepository;
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
                return await CheckAccountReservationLimit(request.AccountId);
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

        public async Task<IList<GlobalRule>> GetAccountRules(long accountId)
        {
            var accountRules = await CheckAccountReservationLimit(accountId);
            return new List<GlobalRule>{accountRules};
        }

        private async Task<GlobalRule> CheckAccountReservationLimit(long accountId)
        {
            var reservations = await _reservationRepository.GetAccountReservations(accountId);

            var maxNumberOfReservations = _options.MaxNumberOfReservations == 0 ? DefaultMaxNumberOfReservations : _options.MaxNumberOfReservations;
            if (reservations.Count >= maxNumberOfReservations)
            {
                return new GlobalRule(new Domain.Entities.GlobalRule
                {
                    Id = 0,
                    Restriction = (byte)AccountRestriction.Account,
                    RuleType = (byte)GlobalRuleType.ReservationLimit
                });
            }

            return null;
        }
    }
}