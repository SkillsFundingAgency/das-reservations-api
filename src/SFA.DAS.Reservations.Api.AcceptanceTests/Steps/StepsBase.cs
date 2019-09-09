using System;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    public class StepsBase
    {
        protected readonly TestData TestData;
        protected readonly IServiceProvider Services;

        public StepsBase(TestData testData, TestServiceProvider serviceProvider)
        {
            TestData = testData;
            Services = serviceProvider;
        }


        [BeforeScenario]
        public void InitialiseTestDatabaseData()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            TestData.Course = new Course
            {
                CourseId = "234",
                Level = 1,
                Title = "Tester"
            };

            dbContext.Courses.Add(TestData.Course);

            TestData.AccountLegalEntity = new AccountLegalEntity
            {
                AccountId = 1,
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "Test Corp",
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                AgreementSigned = true
            };

            dbContext.AccountLegalEntities.Add(TestData.AccountLegalEntity);

            dbContext.SaveChanges();
        }
    }
}