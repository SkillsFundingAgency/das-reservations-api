using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class AccountLegalEntitiesSteps : StepsBase
    {

        public AccountLegalEntitiesSteps(TestData testData,TestResults testResults, TestServiceProvider serviceProvider) : base(testData,testResults, serviceProvider)
        {
        }

        [Given("I am a provider who can act on behalf of an employer")]
        public void GivenIAmAProviderWhoCanActOnBehalfOfAnEmployer()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            
            dbContext.ProviderPermissions.Add(TestData.ProviderPermission);
            
            dbContext.SaveChanges();
        }

        [When("I get the account legal entities")]
        public void WhenIGetTheAccountLegalEntities()
        {
            var controller = Services.GetService<AccountLegalEntitiesController>();

            TestData.ActualResult = controller.GetByProviderId(ProviderId).Result;
        }

        [Then("The non levy employers are returned")]
        public void ThenTheNonLevyEmployersAreReturned()
        {
            var result = ((OkObjectResult)TestData.ActualResult).Value as List<Domain.ProviderPermissions.ProviderPermission>;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        
    }
}