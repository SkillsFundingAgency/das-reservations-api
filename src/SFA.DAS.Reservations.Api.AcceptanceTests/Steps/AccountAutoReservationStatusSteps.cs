using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class CheckAccountAutoReservationStatusSteps : StepsBase
    {
        private bool _canAutoCreateReservations;

        public CheckAccountAutoReservationStatusSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider) : base(testData, testResults, serviceProvider)
        {
        }

        [When(@"I get the account reservation status")]
        public void WhenIGetTheAccountReservationStatus()
        {
            var accountLegalEntitiesController = Services.GetService<AccountLegalEntitiesController>();
            var actionResult = accountLegalEntitiesController.GetAccountReservationStatus(AccountId).Result as OkObjectResult;
            var accountReservationStatus = actionResult.Value as AccountReservationStatus;
            _canAutoCreateReservations = accountReservationStatus.CanAutoCreateReservations;

        }
        
        [Then(@"I am allowed to auto create reservations")]
        public void ThenIAmAllowedToAutoCreateReservations()
        {
            _canAutoCreateReservations.Should().BeTrue();
        }
        
        [Then(@"I am not allowed to auto create reservations")]
        public void ThenIAmNotAllowedToAutoCreateReservations()
        {
            _canAutoCreateReservations.Should().BeFalse();
        }
    }
}
