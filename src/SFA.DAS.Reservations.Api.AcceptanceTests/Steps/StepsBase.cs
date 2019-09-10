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
            TestData.Course = new Course
            {
                CourseId = "1",
                Level = 1,
                Title = "Tester"
            };

            TestData.AccountLegalEntity = new AccountLegalEntity
            {
                AccountId = 1,
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "Test Corp",
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                AgreementSigned = true
            };

            var dbContext = Services.GetService<ReservationsDataContext>();

            var course = dbContext.Courses.SingleOrDefault(c => c.CourseId.Equals(TestData.Course.CourseId));

            if (course == null)
            {
                dbContext.Courses.Add(TestData.Course);
            }

            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(e => e.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));
            
            if (legalEntity == null)
            {
                dbContext.AccountLegalEntities.Add(TestData.AccountLegalEntity);

                dbContext.SaveChanges();
            }
        }
    }
}