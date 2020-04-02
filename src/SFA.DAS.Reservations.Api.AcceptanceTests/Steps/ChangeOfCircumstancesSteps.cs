using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class ChangeOfCircumstancesSteps : StepsBase
    {
        public ChangeOfCircumstancesSteps(
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
                ReservationId = TestData.ReservationId,
                ProviderId = providerId
            };
        }

        [Given(@"I want to change employer to (.*)")]
        public void GivenIWantToChangeEmployerTo(long accountId)
        {
            TestData.Request = new ChangeOfPartyRequest
            {
                ReservationId = TestData.ReservationId,
                AccountId = accountId
            };
        }

        [When("I call change of circumstances")]
        public async Task WhenICallChangeOfCircumstances()
        {
            var request = TestData.Request as ChangeOfPartyRequest;

            var controller = Services.GetService<ReservationsController>();

            TestResults.Result = await controller.Change(request);
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