using System;
using System.Collections.Generic;
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
        protected readonly TestResults TestResults;
        protected readonly IServiceProvider Services;

        public StepsBase(TestData testData, TestResults testResults, TestServiceProvider serviceProvider)
        {
            TestData = testData;
            TestResults = testResults;
            Services = serviceProvider;
            UserId = Guid.NewGuid();
        }


        [BeforeScenario()]
        public void InitialiseTestDatabaseData()
        {
            TestData.Course = new Course
            {
                CourseId = "1",
                Level = 1,
                Title = "Tester",
                ReservationRule = new List<Rule>()
            };           

            TestData.AccountLegalEntity = new AccountLegalEntity
            {
                AccountId = AccountId,
                AccountLegalEntityId = AccountLegalEntityId,
                AccountLegalEntityName = "Test Corp",
                AgreementSigned = true
            };
            
            TestData.ProviderPermission = new ProviderPermission
            {
                AccountId = AccountId,
                UkPrn = ProviderId,
                CanCreateCohort = true,
                AccountLegalEntityId = AccountLegalEntityId
            };
            
            TestData.UserId = UserId;

            var dbContext = Services.GetService<ReservationsDataContext>();

            if (dbContext.Courses.Find(TestData.Course.CourseId) == null)
            {
                dbContext.Courses.Add(TestData.Course);
            }

            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(e => e.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));
            
            if (legalEntity == null)
            {
                dbContext.AccountLegalEntities.Add(TestData.AccountLegalEntity);

                dbContext.SaveChanges();
            }

            var anotherAccountLegalEntity = new AccountLegalEntity
            {
                AccountId = AccountId + 1,
                AccountLegalEntityId = AccountLegalEntityId + 100,
                AccountLegalEntityName = "Pass Mark Corp",
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                AgreementSigned = true
            };

            legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(e => e.AccountLegalEntityId.Equals(anotherAccountLegalEntity.AccountLegalEntityId));

            if (legalEntity == null)
            {
                dbContext.AccountLegalEntities.Add(anotherAccountLegalEntity);

                dbContext.SaveChanges();
            }

            TestData.Account = new Account
            {
                Id = AccountId,
                Name = "Test Account"
            };

            var account = dbContext.Accounts.Find(AccountId);

            if (account == null)
            {
                dbContext.Accounts.Add(TestData.Account);
                dbContext.SaveChanges();
            }
        }

    }
}
