using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourse.SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.BulkValidate
{
    public class NonLevyReservationRequestCommandHandler
        : IRequestHandler<NonLevyReservationRequestCommand, BulkValidationResults>
    {
        private readonly IAccountReservationService _accountReservationService;
        private readonly IGlobalRulesService _globalRulesService;
        private readonly IUnitOfWorkContext _context;
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;
        private readonly IAccountsService _accountService;
        private readonly IMediator _mediator;

        public NonLevyReservationRequestCommandHandler(IAccountReservationService accountReservationService,
            IGlobalRulesService globalRulesService,
            IUnitOfWorkContext context, IAccountLegalEntitiesService accountLegalEntitiesService, IAccountsService accountsService
            ,IMediator mediator)
        {
            _accountReservationService = accountReservationService;
            _globalRulesService = globalRulesService;
            _context = context;
            _accountLegalEntitiesService = accountLegalEntitiesService;
            _accountService = accountsService;
            _mediator = mediator;
        }

        public async Task<BulkValidationResults> Handle(NonLevyReservationRequestCommand reservationRequestCommand, CancellationToken cancellationToken)
        {
            var result = new BulkValidationResults();
            result.ValidationErrors = new List<BulkValidation>();

            var accountLegalEntity =
                await _accountLegalEntitiesService.GetAccountLegalEntity(reservationRequestCommand.AccountLegalEntityId);

            if (accountLegalEntity.IsLevy)
            {
                return new BulkValidationResults();
            }


            if (await FailedGlobalRuleValidation())
            {
                result.ValidationErrors.Add(new BulkValidation { Reason = "Failed global rule validation", FileLevelError = true });
                return result;
            }

            if (await FailedAccountRuleValidation(reservationRequestCommand.AccountId))
            {
                result.ValidationErrors.Add(new BulkValidation { Reason = "Failed account rule validation", FileLevelError = true });
                return result;
            }

            var request = reservationRequestCommand.Request;

            var globalRule = await _globalRulesService.CheckReservationAgainstRules(request);

            if (globalRule != null)
            {
                var shouldReturnError = false;
                switch (globalRule.Restriction)
                {
                    case AccountRestriction.All:
                    case AccountRestriction.Account:
                        shouldReturnError = true;
                        break;
                    case AccountRestriction.Levy:
                        if (request.IsLevyAccount)
                        {
                            shouldReturnError = true;
                        }

                        break;
                    case AccountRestriction.NonLevy:
                        if (!request.IsLevyAccount)
                        {
                            shouldReturnError = true;
                        }

                        break;
                }

                if (shouldReturnError)
                {
                    result.ValidationErrors.Add(new BulkValidation { Reason = globalRule.RestrictionText });
                }
            }

            if (!accountLegalEntity.IsLevy && !accountLegalEntity.AgreementSigned)
            {
                result.ValidationErrors.Add(new BulkValidation { Reason = "Agreement not signed" });
            }

            if (!await ValidateStartDate(reservationRequestCommand.AccountId, request.StartDate.Value))
            {
                // TODO : discuss this - not sure about this.
                result.ValidationErrors.Add(new BulkValidation { Reason = "start date is not valid :" + request.StartDate.Value });
            }

            if (!await ValidateCourse(request.CourseId))
            {
                result.ValidationErrors.Add(new BulkValidation { Reason = "course is not valid:" + request.CourseId });
            }


            return result;
        }

        private async Task<bool> ValidateCourse(string courseId)
        {
            var response = await _mediator.Send(new GetCourseQuery() { CourseId = courseId });

            return response != null;
        }

        private async Task<bool> ValidateStartDate(long accountLegalEntityId, DateTime startDate)
        {
            var availableDatesResult = await _mediator.Send(new GetAvailableDatesQuery { AccountLegalEntityId = accountLegalEntityId });

            return availableDatesResult.AvailableDates.Any(x => startDate >= x.StartDate && startDate <= x.EndDate);
        }

        private async Task<bool> FailedAccountRuleValidation(long accountId)
        {
            var accountFundingRulesApiResponse = await _mediator.Send(new GetAccountRulesQuery { AccountId = accountId });
            //_rulesService.GetAccountFundingRules();
            if (accountFundingRulesApiResponse.GlobalRules.Any(c => c != null && c.RuleType == GlobalRuleType.ReservationLimit) &&
                accountFundingRulesApiResponse.GlobalRules.Count(c => c.RuleType == GlobalRuleType.ReservationLimit) > 0)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> FailedGlobalRuleValidation()
        {
            var globalRulesApiResponse = await _mediator.Send(new GetRulesQuery());
            //_globalRulesService.GetAllRules();
            if (globalRulesApiResponse.GlobalRules != null
    && globalRulesApiResponse.GlobalRules.Any(c => c != null && c.RuleType == GlobalRuleType.FundingPaused)
    && globalRulesApiResponse.GlobalRules.Count(c => c.RuleType == GlobalRuleType.FundingPaused && DateTime.UtcNow >= c.ActiveFrom) > 0)
            {
                return true;
                // result.FailedGlobalRuleValidation = true;
            }

            return false;
        }



        private async Task<bool> ApprenticeshipCountExceedsRemainingReservations(BulkValidateCommand bulkRequest, BulkValidationResults result)
        {
            var account = await _accountService.GetAccount(bulkRequest.AccountId);

            var reservationLimit = account.ReservationLimit;
            var remainingReservation = await _accountReservationService.GetRemainingReservations(bulkRequest.AccountId, reservationLimit);

            if (remainingReservation < bulkRequest.Requests.Count)
            {
                return true;
            }

            return false;
        }
    }
}
