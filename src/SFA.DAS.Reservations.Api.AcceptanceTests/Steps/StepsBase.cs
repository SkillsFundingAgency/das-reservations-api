using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Api.AcceptanceTests.ValueComparers;
using SFA.DAS.Reservations.Api.AcceptanceTests.ValueRetrievers;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

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
        private readonly ReservationsDataContext _dbContext;

        public StepsBase(TestData testData, TestResults testResults, TestServiceProvider serviceProvider)
        {
            TestData = testData;
            TestResults = testResults;
            Services = serviceProvider;
            UserId = Guid.NewGuid();

            _dbContext = Services.GetService<ReservationsDataContext>();
        }
        
        [BeforeScenario]
        public void InitialiseTestDatabaseData()
        {
            ConfigureCustomValueRetrievers();
            ConfigureCustomValueComparers();

            ArrangeTestData();

            AddCoursesToDb();
            AddAccountsToDb();
            AddAccountLegalEntitiesToDb();
        }

        private void ConfigureCustomValueRetrievers()
        {
            Service.Instance.ValueRetrievers.Replace<DateTimeValueRetriever, CustomDateTimeValueRetriever>();
            Service.Instance.ValueRetrievers.Replace<NullableDateTimeValueRetriever, CustomNullableDateTimeValueRetriever>();
            Service.Instance.ValueRetrievers.Replace<ShortValueRetriever, CustomShortValueRetriever>();
        }

        private void ConfigureCustomValueComparers()
        {
            Service.Instance.ValueComparers.Register<CustomShortValueComparer>();
            Service.Instance.ValueComparers.Register(new CustomDateTimeComparer(new CustomDateTimeValueRetriever()));
        }

        private void ArrangeTestData()
        {
            TestData.Course = new Course
            {
                CourseId = "1",
                Level = 1,
                Title = "Tester",
                ReservationRule = new List<Rule>()
            };           
            
            var framework = new Course
            {
                CourseId = "1-1-1",
                Level = 1,
                Title = "Tester",
                ReservationRule = new List<Rule>()
            };
            
            TestData.Account = new Account
            {
                Id = AccountId,
                Name = "Test Account"
            };
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
        }

        private void AddCoursesToDb()
        {            if (_dbContext.Courses.Find(TestData.Course.CourseId) == null)
            {
                _dbContext.Courses.Add(TestData.Course);
            }

            if (_dbContext.Courses.Find(framework.CourseId) == null)
            {
                _dbContext.Courses.Add(framework);
            }        {
        }

        private void AddAccountsToDb()
        {
            var accounts = new List<Account>
            {
                TestData.Account,
                new Account {Id = AccountId + 1, Name = "Account 2 non-levy"},
                new Account {Id = AccountId + 10, Name = "Account 11 levy", IsLevy = true},
                new Account {Id = AccountId + 11, Name = "Account 12 levy", IsLevy = true}
            };

            foreach (var account in accounts)
            {
                if(_dbContext.Accounts.Find(account.Id) != null)
                    continue;

                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
            }
        }

        private void AddAccountLegalEntitiesToDb()
        {
            var accountLegalEntities = new List<AccountLegalEntity>
            {
                TestData.AccountLegalEntity,
                new AccountLegalEntity
                {
                    AccountId = AccountId + 1,
                    AccountLegalEntityId = 2,
                    AccountLegalEntityName = "ALE 2 - non levy",
                    AgreementSigned = true
                },
                new AccountLegalEntity
                {
                    AccountId = AccountId + 10,
                    AccountLegalEntityId = 10,
                    AccountLegalEntityName = "ALE 10 - levy",
                    AgreementSigned = true,
                },
                new AccountLegalEntity
                {
                    AccountId = AccountId + 11,
                    AccountLegalEntityId = 20,
                    AccountLegalEntityName = "ALE 20 - levy",
                    AgreementSigned = true
                },
                new AccountLegalEntity
                {
                    AccountId = AccountId + 1,
                    AccountLegalEntityId = AccountLegalEntityId + 100,
                    AccountLegalEntityName = "Pass Mark Corp",
                    AgreementSigned = true
                }
            };

            foreach (var accountLegalEntity in accountLegalEntities)
            {
                if(_dbContext.AccountLegalEntities.SingleOrDefault(e => 
                       e.AccountLegalEntityId.Equals(accountLegalEntity.AccountLegalEntityId)) != null)
                    continue;

                _dbContext.AccountLegalEntities.Add(accountLegalEntity);
                _dbContext.SaveChanges();
            }
        }
    }
}
