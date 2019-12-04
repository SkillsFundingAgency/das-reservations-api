using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class GetAccountLegalEntitiesSteps : StepsBase
    {
        private IEnumerable<Domain.AccountLegalEntities.AccountLegalEntity> _accountLegalEntitiesList;

        public GetAccountLegalEntitiesSteps(TestData testData, TestServiceProvider serviceProvider) : base(testData, serviceProvider)
        {
        }

        [Given(@"I have a legal entity with unsigned agreement")]
        public void GivenIHaveALegalEntityWithUnsignedAgreement()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
           
            var legalEntity = dbContext.AccountLegalEntities.FirstOrDefault(e => e.AccountId.Equals(TestData.AccountLegalEntity.AccountId) && !e.AgreementSigned);
            
            if (legalEntity == null)
            {
                var newLegalEntity = new AccountLegalEntity
                {
                    AccountId = AccountId,
                    AccountLegalEntityId = AccountLegalEntityId + 1,
                    AccountLegalEntityName = "Test Corp 2",
                    AgreementType = AgreementType.NonLevyExpressionOfInterest,
                    AgreementSigned = false
                };

                dbContext.AccountLegalEntities.Add(newLegalEntity);

                dbContext.SaveChanges();
            }
        }
        
        [Given(@"I have a legal entity with signed agreement")]
        public void GivenIHaveALegalEntityWithSignedAgreement()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
           
            var legalEntity = dbContext.AccountLegalEntities.FirstOrDefault(e => e.AccountId.Equals(TestData.AccountLegalEntity.AccountId) && e.AgreementSigned);
            
            if (legalEntity == null)
            {
                var newLegalEntity = new AccountLegalEntity
                {
                    AccountId = AccountId,
                    AccountLegalEntityId = AccountLegalEntityId + 2,
                    AccountLegalEntityName = "Test Corp 3",
                    AgreementType = AgreementType.NonLevyExpressionOfInterest,
                    AgreementSigned = true
                };

                dbContext.AccountLegalEntities.Add(newLegalEntity);

                dbContext.SaveChanges();
            }
        }


        [When(@"I get my legal entities attached to the account")]
        public void WhenIGetMyLegalEntitiesAttachedToTheAccount()
        {
            var accountLegalEntitiesController = Services.GetService<AccountLegalEntitiesController>();

            var actionResult = accountLegalEntitiesController.GetByAccountId(AccountId).Result as OkObjectResult;
            _accountLegalEntitiesList = actionResult.Value as IEnumerable<Domain.AccountLegalEntities.AccountLegalEntity>;
        }

        [Then(@"all legal entities for my account are returned")]
        public void ThenAllLegalEntitiesForMyAccountAreReturned()
        {
            Assert.True(
                _accountLegalEntitiesList.All(ale => ale.AccountId.Equals(TestData.AccountLegalEntity.AccountId)));

        }

    }
}
