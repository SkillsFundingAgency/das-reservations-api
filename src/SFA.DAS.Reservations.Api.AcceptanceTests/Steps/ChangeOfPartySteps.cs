using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class ChangeOfPartySteps : StepsBase
    {
        public ChangeOfPartySteps(
            TestData testData, 
            TestResults testResults, 
            TestServiceProvider serviceProvider) 
            : base(testData, testResults, serviceProvider)
        {
        }

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

        [When("I call change of circumstances")]
        public async Task WhenICallChangeOfCircumstances()
        {
            var request = TestData.Request as ChangeOfPartyRequest;

            var controller = Services.GetService<ReservationsController>();

            TestResults.Result = await controller.Change(TestData.ReservationId, request);
        }

        [Then("an http status code of (.*) is returned")]
        public void ThenAnHttpStatusCodeIsReturned(int httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case (int)HttpStatusCode.BadRequest:
                    TestResults.Result.Should().BeOfType<BadRequestResult>();
                    break;
                default:
                    Assert.Fail("http status code not supported");
                    break;
            }
        }
    }
}