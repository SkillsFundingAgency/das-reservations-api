using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
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
        private long? _transferSenderId = null;
        private bool _canAutoCreateReservations;

        public CheckAccountAutoReservationStatusSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider) : base(testData, testResults, serviceProvider)
        {
        }

        [Given(@"I have transfer funds")]
        public void GivenIHaveTransferFunds()
        {
            _transferSenderId = 123234;
            var dbContext = Services.GetService<ReservationsDataContext>();
            var account = new Account
            {
                Id = _transferSenderId.Value,
                Name = "Test Account",
                IsLevy = true    
            };
            var entity = new AccountLegalEntity
            {
                AccountId = _transferSenderId.Value,
                AccountLegalEntityId = AccountLegalEntityId + 4,
                AccountLegalEntityName = "Test 4 Corp",
                AgreementSigned = true
                
            };
            dbContext.Accounts.Add(account);
            dbContext.AccountLegalEntities.Add(entity);
            dbContext.SaveChanges();

        }
        
        [When(@"I get the account reservation status")]
        public void WhenIGetTheAccountReservationStatus()
        {
            var accountLegalEntitiesController = Services.GetService<AccountLegalEntitiesController>();
            var actionResult = accountLegalEntitiesController.GetAccountReservationStatus(AccountId, _transferSenderId).Result as OkObjectResult;
            var accountReservationStatus = actionResult.Value as AccountReservationStatus;
            _canAutoCreateReservations = accountReservationStatus.CanAutoCreateReservations;

        }
        
        [Then(@"I am allowed to auto create reservations")]
        public void ThenIAmAllowedToAutoCreateReservations()
        {
            Assert.True(_canAutoCreateReservations);
        }
        
        [Then(@"I am not allowed to auto create reservations")]
        public void ThenIAmNotAllowedToAutoCreateReservations()
        {
            Assert.False(_canAutoCreateReservations);
        }
    }
}
