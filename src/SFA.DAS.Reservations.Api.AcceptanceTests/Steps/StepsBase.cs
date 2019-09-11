using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    
    public class StepsBase
    {
        protected const long AccountId = 1;
        protected const long AccountLegalEntityId = 1;
        protected const uint ProviderId = 15214;
        protected Guid UserId;
        protected readonly TestData TestData;
        protected readonly IServiceProvider Services;

        public StepsBase(TestData testData, TestServiceProvider serviceProvider)
        {
            TestData = testData;
            Services = serviceProvider;
            UserId = Guid.NewGuid();
        }


        [BeforeScenario()]
        public void InitialiseTestDatabaseData()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            TestData.Course = new Course
            {
                CourseId = "234",
                Level = 1,
                Title = "Tester"
            };
            if (dbContext.Courses.Find("234") == null)
            {

                dbContext.Courses.Add(TestData.Course);
            }

            TestData.AccountLegalEntity = new AccountLegalEntity
            {
                AccountId = AccountId,
                AccountLegalEntityId = AccountLegalEntityId,
                AccountLegalEntityName = "Test Corp",
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                AgreementSigned = true
            };

            if (dbContext.AccountLegalEntities.FirstOrDefault(c => c.AccountLegalEntityId.Equals(AccountLegalEntityId)) == null)
            {
                dbContext.AccountLegalEntities.Add(TestData.AccountLegalEntity);
            }
            


            dbContext.SaveChanges();
        }

    }
}