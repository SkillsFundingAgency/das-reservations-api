using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenCheckingAccountLevelRules
    {
        private Mock<IAccountReservationService> _repository;
        private GlobalRulesService _globalRulesService;
        private Domain.Entities.GlobalRule _globalRule;
        private Mock<IAccountsService> _accountService;
        private const long ExpectedAccountId = 534542143;

        [SetUp]
        public void Arrange()
        {
            _globalRule = new Domain.Entities.GlobalRule
            {
                Id = 0,
                Restriction = (byte)AccountRestriction.Account,
                RuleType = (byte)GlobalRuleType.ReservationLimit
            };
            _repository = new Mock<IAccountReservationService>();
            _repository.Setup(x => x.GetAccountReservations(ExpectedAccountId)).ReturnsAsync(new List<Reservation>{new Reservation(
                Guid.NewGuid(), 
                ExpectedAccountId,
                DateTime.UtcNow.Date, 
                2,
                "Name")});
            _accountService = new Mock<IAccountsService>();
            _accountService.Setup(x => x.GetAccount(It.IsAny<long>()))
                .ReturnsAsync(new Domain.Account.Account(ExpectedAccountId, false, "test", 1));

            _globalRulesService = new GlobalRulesService(Mock.Of<IGlobalRuleRepository>(), Mock.Of<IOptions<ReservationsConfiguration>>(), _repository.Object, _accountService.Object);
        }

        [Test]
        public async Task Then_The_GlobalRules_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _globalRulesService.GetAccountRules(ExpectedAccountId);

            //Assert
            _repository.Verify(x => x.GetAccountReservations(ExpectedAccountId),Times.Once);
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Then_The_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _globalRulesService.GetAccountRules(ExpectedAccountId);

            //Assert
            Assert.IsAssignableFrom<List<GlobalRule>>(actual);
            Assert.IsNotEmpty(actual);
            var actualRule = actual.FirstOrDefault();
            Assert.IsNotNull(actualRule);
            Assert.AreEqual(_globalRule.Id, actualRule.Id);
            Assert.AreEqual(_globalRule.RuleType, (byte)actualRule.RuleType);
            Assert.AreEqual(_globalRule.Restriction, (byte)actualRule.Restriction);
        }
    }
}
