using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Rules;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class AcknowledgeRulesSteps : StepsBase
    {
        public AcknowledgeRulesSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider) : base(testData, testResults, serviceProvider)
        {
        }

        [Given(@"I have upcoming rules")]
        public void GivenIHaveUpcomingRules()
        {
           var dbContext = Services.GetService<ReservationsDataContext>();

            dbContext.GlobalRules.Add(new Domain.Entities.GlobalRule
            {
                Id = 1,
                RuleType = (byte)GlobalRuleType.FundingPaused,
                ActiveFrom = DateTime.UtcNow.AddMonths(1),
                ActiveTo = DateTime.UtcNow.AddMonths(2),
                Restriction = (byte)AccountRestriction.All,
            });
            dbContext.SaveChanges();
        }

        [Given(@"there is a restriction for non-levy accounts")]
        public void GivenThereIsARestrictionForNon_LevyAccounts()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            dbContext.GlobalRules.Add(new Domain.Entities.GlobalRule
            {
                Id = 1,
                RuleType = (byte)GlobalRuleType.FundingPaused,
                ActiveFrom = DateTime.UtcNow.AddMonths(-1),
                ActiveTo = DateTime.UtcNow.AddMonths(2),
                Restriction = (byte)AccountRestriction.NonLevy,
            });
            dbContext.SaveChanges();
        }


        [When(@"I acknowledge a rule as being read as a provider")]
        public void WhenIAcknowledgeARuleAsBeingRead()
        {
            MarkRuleAsRead(ProviderId.ToString());
        }


        [When(@"I acknowledge a rule as being read as a employer")]
        public void WhenIAcknowledgeARuleAsBeingReadAsAEmployer()
        {
            MarkRuleAsRead(UserId.ToString());
        }

        private void MarkRuleAsRead(string id)
        {
            var controller = Services.GetService<RulesController>();

            var result = (OkObjectResult) Task.FromResult(controller.All().Result).Result;
            var rules = (GetRulesResult) result.Value;
            var ruleId = rules.GlobalRules.FirstOrDefault().Id;

            controller.AcknowledgeRuleAsRead(new CreateUserRuleAcknowledgementCommand
            {
                Id = id,
                RuleId = ruleId,
                TypeOfRule = RuleType.GlobalRule
            }).Wait();
        }


        [Then(@"it is not returned in the list of upcoming rules for the (.*)")]
        public void ThenItIsNotReturnedInTheListOfUpcomingRules(string type)
        {
            var controller = Services.GetService<RulesController>();
            var result = (OkObjectResult) Task.FromResult(controller.All().Result).Result;
            var rules = (GetRulesResult) result.Value;
            var globalRule = rules.GlobalRules.FirstOrDefault();

            var userRuleAcknowledgement = globalRule.UserRuleAcknowledgements.FirstOrDefault();

            if (type.Equals("provider", StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.AreEqual(ProviderId, userRuleAcknowledgement.UkPrn);
            }

            if (type.Equals("employer", StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.AreEqual(UserId, userRuleAcknowledgement.UserId);
            }
        }
    }
}
