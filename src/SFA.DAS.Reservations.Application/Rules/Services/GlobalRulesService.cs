using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class GlobalRulesService : IGlobalRulesService
    {
        private readonly IGlobalRuleRepository _repository;
        private readonly IAccountReservationService _reservationService;
        private readonly IAccountsService _accountService;
        private readonly ReservationsConfiguration _options;
        ILogger<GlobalRulesService> _logger;

        public GlobalRulesService(
            IGlobalRuleRepository repository, 
            IOptions<ReservationsConfiguration> options,
            IAccountReservationService reservationService, 
            IAccountsService accountService,
            ILogger<GlobalRulesService> logger
            )
        {
            _repository = repository;
            _reservationService = reservationService;
            _accountService = accountService;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<IList<GlobalRule>> GetAllRules()
        {
            var result = await _repository.GetAll();

            return result.Select(globalRule => new GlobalRule(globalRule)).ToList();
        }

        public async Task<IList<GlobalRule>> GetActiveRules(DateTime fromDate)
        {
            var result = await _repository.FindActive(fromDate);

            return result.Select(globalRule => new GlobalRule(globalRule)).ToList();
        }

        public async Task<GlobalRule> CheckReservationAgainstRules(IReservationRequest request)
        {
            var resultsList = await _repository.FindActive(request.CreatedDate);

            resultsList = resultsList
                .Where(r => r.RuleType != (byte)GlobalRuleType.DynamicPause || r.ActiveTo > request.StartDate)
                .Where(r => !r.GlobalRuleAccountExemptions?.Any(e => e.AccountId == request.AccountId) ?? true)
                .ToList();

            if (resultsList == null || !resultsList.Any())
            {
                return await CheckAccountReservationLimit(request.AccountId, request.IsLevyAccount);
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
                    request.IsLevyAccount,
                    request.TransferSenderAccountId);

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

        public async Task<bool> HasReachedReservationLimit(long accountId, bool isLevyReservation = false)
        {
            var rule = await CheckAccountReservationLimit(accountId, isLevyReservation);

            return rule != null;
        }

        private async Task<int?> GetReservationLimit(long accountId)
        {
            var account = await _accountService.GetAccount(accountId);

            return account.ReservationLimit;
        }

        private async Task<GlobalRule> CheckAccountReservationLimit(long accountId, bool isLevyReservation = false)
        {
            if (isLevyReservation)
            {
                return null;
            }
            
            var maxNumberOfReservations = await GetReservationLimit(accountId);

            if (maxNumberOfReservations == null)
            {
                return null;
            }

            var remainingReservations = await _reservationService.GetRemainingReservations(accountId, maxNumberOfReservations.Value);

            _logger.LogWarning("Reset reservation date:" +
                (_options.ResetReservationDate.HasValue 
                ? _options.ResetReservationDate.Value.ToString("dd/MM/yyyy")
                : "no reset reservation date set"));

            if (remainingReservations <= 0)
            {
                _logger.LogWarning($"Account: {accountId} has {remainingReservations} remainingReservations");
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