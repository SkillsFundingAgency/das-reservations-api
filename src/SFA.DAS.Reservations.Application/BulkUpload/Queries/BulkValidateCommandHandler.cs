using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.BulkUpload.Queries
{
    public class BulkValidateCommandHandler : IRequestHandler<BulkValidateCommand, BulkValidationResults>
    {
        private readonly IAccountReservationService _accountReservationService;
        private readonly IGlobalRulesService _globalRulesService;
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;
        private readonly IAccountsService _accountService;
        private readonly IMediator _mediator;
        private readonly Dictionary<long, AccountLegalEntity> _cachedAccountLegalEntities;
        private readonly ReservationsConfiguration _configuration;
        private readonly ILogger<BulkValidateCommandHandler> _logger;
        private readonly ICurrentDateTime _currentDateTime;

        public BulkValidateCommandHandler(IAccountReservationService accountReservationService,
            IGlobalRulesService globalRulesService,
            IAccountLegalEntitiesService accountLegalEntitiesService, IAccountsService accountsService,
            IMediator mediator,
            IOptions<ReservationsConfiguration> options,
            ILogger<BulkValidateCommandHandler> logger,
            ICurrentDateTime currentDateTime)
        {
            _accountReservationService = accountReservationService;
            _globalRulesService = globalRulesService;
            _accountLegalEntitiesService = accountLegalEntitiesService;
            _accountService = accountsService;
            _mediator = mediator;
            _cachedAccountLegalEntities = new Dictionary<long, AccountLegalEntity>();
            _configuration = options.Value;
            _logger = logger;
            _currentDateTime = currentDateTime;
        }

        public async Task<BulkValidationResults> Handle(BulkValidateCommand bulkRequest, CancellationToken cancellationToken)
        {
            var result = new BulkValidationResults
            {
                ValidationErrors = new List<BulkValidation>()
            };

            // Only run validation for valid agreement ids - which have values.
            var validAgreementIds = bulkRequest.Requests.Where(x => x.AccountLegalEntityId.HasValue);

            var groups = validAgreementIds.GroupBy(x => new {AccountLegalEntityId =  x.AccountLegalEntityId.Value, x.TransferSenderAccountId });

            foreach (var group in groups)
            {
                var accountLegalEntity = await GetAccountLegalEntity(group.Key.AccountLegalEntityId);

                if (accountLegalEntity == null)
                {
                    continue;
                }

                if (!accountLegalEntity.AgreementSigned || accountLegalEntity.IsLevy ||
                    group.Key.TransferSenderAccountId.HasValue)
                {
                    continue;
                }
                
                if (await FailedGlobalRuleValidation())
                {
                    AddErrorForAllRows(result, group, "Failed global rule validation");
                    return result;
                }

                if (await ApprenticeshipCountExceedsRemainingReservations(accountLegalEntity.AccountId, group.Count()))
                {
                    AddErrorForAllRows(result, group, "The employer has reached their <b>reservations limit</b>. Contact the employer.");
                }
                else if (await FailedAccountRuleValidation(accountLegalEntity.AccountId))
                {
                    AddErrorForAllRows(result, group, "Reservation failed for account.");
                }
                else
                {
                    foreach (var validateRequest in group)
                    {
                        if (accountLegalEntity == null || !validateRequest.StartDate.HasValue || !validateRequest.ProviderId.HasValue || string.IsNullOrWhiteSpace(validateRequest.CourseId))
                        {
                            continue;
                        }
                        
                        var dateFailureError = await FailedStartDateValidation(validateRequest.StartDate, validateRequest.AccountLegalEntityId.Value, accountLegalEntity.AccountId);
                        
                        if (!string.IsNullOrWhiteSpace(dateFailureError))
                        {
                            result.ValidationErrors.Add(new BulkValidation { Reason = dateFailureError, RowNumber = validateRequest.RowNumber });
                        }
                        else
                        {
                            // calling legacy service, we may be able to remove this, but will need to investigate further.
                            var reservationRule = await _globalRulesService.CheckReservationAgainstRules(GetBulkCheckReservationAgainRule(validateRequest, accountLegalEntity));
                            if (reservationRule == null)
                            {
                                continue;
                            }
                            
                            _logger.LogInformation("Failed reservation rule for reason : {Reason}.", reservationRule.RuleTypeText);
                            result.ValidationErrors.Add(new BulkValidation { Reason = "Failed reservation rules", RowNumber = validateRequest.RowNumber });
                        }
                    }
                }
            }

            return result;
        }

        private static void AddErrorForAllRows(BulkValidationResults result, IGrouping<object, BulkValidateRequest> group, string error)
        {
            foreach (var row in group)
            {
                result.ValidationErrors.Add(new BulkValidation { Reason = error, RowNumber = row.RowNumber });
            }
        }

        private static IReservationRequest GetBulkCheckReservationAgainRule(BulkValidateRequest validateRequest, AccountLegalEntity accountLegalEntity)
        {
            return new BulkCheckReservationAgainRule
            {
                Id = Guid.NewGuid(),
                AccountId = accountLegalEntity.AccountId,
                AccountLegalEntityId = accountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = accountLegalEntity.AccountLegalEntityName,
                CourseId = validateRequest.CourseId,
                CreatedDate = validateRequest.CreatedDate,
                IsLevyAccount = accountLegalEntity.IsLevy,
                ProviderId = validateRequest.ProviderId,
                StartDate = validateRequest.StartDate,
                TransferSenderAccountId = validateRequest.TransferSenderAccountId,
                UserId = validateRequest.UserId
            };
        }

        private async Task<AccountLegalEntity> GetAccountLegalEntity(long accountLegalEntityId)
        {
            if (_cachedAccountLegalEntities.ContainsKey(accountLegalEntityId))
            {
                return _cachedAccountLegalEntities.GetValueOrDefault(accountLegalEntityId);
            }

            var accountLegalEntity = await _accountLegalEntitiesService.GetAccountLegalEntity(accountLegalEntityId);
            _cachedAccountLegalEntities.Add(accountLegalEntityId, accountLegalEntity);
            return accountLegalEntity;
        }

        private async Task<string> FailedStartDateValidation(DateTime? startDate, long accountLegalEntityId, long accountId)
        {
            var availableDates = await _mediator.Send(new GetAvailableDatesQuery { AccountLegalEntityId = accountLegalEntityId });
            if (availableDates == null || !availableDates.AvailableDates.Any())
            {
                return "No available dates for reservation found";
            }
            
            var response = await _mediator.Send(new GetAccountRulesQuery { AccountId = accountId });
            var activeRule = response?.GlobalRules?.Where(r => r != null)?.MinBy(x => x.ActiveFrom);

            if (activeRule != null)
            {
                _logger.LogInformation($"Found an active rule {activeRule.RuleTypeText} for accountId {accountId} with ActiveTo is {activeRule.ActiveTo?.ToString() ?? "Null"}");
            }
            
            var possibleDates = activeRule == null
                 ? availableDates.AvailableDates.OrderBy(x => x.StartDate)
                 : availableDates.AvailableDates.Where(x => x.StartDate >= activeRule.ActiveTo).Select(x => x).OrderBy(x => x.StartDate);

            if (possibleDates == null || !possibleDates.Any())
            {
                return "No reservation dates available for account";
            }

            var possibleStartDates = possibleDates?.Select(x => x.StartDate)?.OrderBy(model => model);

            if (possibleStartDates == null || !possibleStartDates.Any())
            {
                return "No reservation dates found for account";
            }
            
            var previousMonthDate = _currentDateTime.GetDate().AddMonths(-1);
            var firstDateOfPreviousMonth = new DateTime(previousMonthDate.Year, previousMonthDate.Month, 1);
            
            if (startDate.Value < firstDateOfPreviousMonth)
            {
                return $"The start date cannot be before {firstDateOfPreviousMonth:dd/MM/yyyy}. You can only backdate a reservation for 1 month.";
            }

            if (startDate.Value < possibleStartDates.Min())
            {
                return $"The start for this learner cannot be before {possibleStartDates.Min():dd/MM/yyyy} (first month of the window). You cannot backdate reserve funding.";
            }
            
            if (startDate.Value > possibleStartDates.Max())
            {
                var expiryPeriodInMonths = _configuration.ExpiryPeriodInMonths;

                var expiryMonths = expiryPeriodInMonths == 0 ? 6 : expiryPeriodInMonths;

                if (expiryMonths > 12)
                {
                    expiryMonths = 12;
                }

                var possibleEndDate = possibleStartDates.Max();
                var maxDate = new DateTime(possibleEndDate.Year, possibleEndDate.Month, DateTime.DaysInMonth(possibleEndDate.Year, possibleEndDate.Month));
                return $"The start for this learner cannot be after {maxDate:dd/MM/yyyy} (last month of the window) You cannot reserve funding more than {expiryMonths} months in advance.";
            }
            
            return null;
        }

        private async Task<bool> FailedAccountRuleValidation(long accountId)
        {
            var accountFundingRulesApiResponse = await _mediator.Send(new GetAccountRulesQuery { AccountId = accountId });
            if (accountFundingRulesApiResponse?.GlobalRules?.Any(c => c != null && c.RuleType == GlobalRuleType.ReservationLimit) ?? false)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> FailedGlobalRuleValidation()
        {
            var globalRulesApiResponse = await _mediator.Send(new GetRulesQuery());
            if (globalRulesApiResponse?.GlobalRules != null
                 && globalRulesApiResponse.GlobalRules.Any(c => c != null && c.RuleType == GlobalRuleType.FundingPaused && DateTime.UtcNow >= c.ActiveFrom))
            {
                return true;
            }

            return false;
        }

        private async Task<bool> ApprenticeshipCountExceedsRemainingReservations(long accountId, int numberOfNewReservation)
        {
            var account = await _accountService.GetAccount(accountId);

            var reservationLimit = account.ReservationLimit;
            var remainingReservation = await _accountReservationService.GetRemainingReservations(accountId, reservationLimit ?? 0);

            return remainingReservation < numberOfNewReservation;
        }

        public class BulkCheckReservationAgainRule : IReservationRequest
        {
            public Guid Id {get; set;}

            public long AccountId {get; set;}

            public DateTime? StartDate {get; set;}

            public string CourseId {get; set;}

            public uint? ProviderId {get; set;}

            public long AccountLegalEntityId {get; set;}

            public string AccountLegalEntityName {get; set;}

            public bool IsLevyAccount {get; set;}

            public DateTime CreatedDate {get; set;}

            public long? TransferSenderAccountId {get; set;}

            public Guid? UserId {get; set;}
        }
    }
}
