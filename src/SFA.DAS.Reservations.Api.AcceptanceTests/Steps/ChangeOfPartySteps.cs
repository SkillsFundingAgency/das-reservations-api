using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class ChangeOfPartySteps(
        TestData testData,
        TestResults testResults,
        TestServiceProvider serviceProvider)
        : StepsBase(testData, testResults, serviceProvider)
    {
        [Given(@"I want to change provider to (.*)")]
        public void GivenIWantToChangeProviderTo(uint providerId)
        {
            TestData.Request = new ChangeOfPartyRequest
            {
                ProviderId = providerId
            };
        }

        [Given(@"I want to change account legal entity to (.*)")]
        public void GivenIWantToChangeAccountLegalEntityTo(long accountLegalEntityId)
        {
            TestData.Request = new ChangeOfPartyRequest
            {
                AccountLegalEntityId = accountLegalEntityId
            };
        }

        [When("I call change of party")]
        public async Task WhenICallChangeOfParty()
        {
            var request = TestData.Request as ChangeOfPartyRequest;

            var controller = Services.GetService<ReservationsController>();

            TestResults.Result = await controller.Change(TestData.ReservationId, request);
        }
    }
}