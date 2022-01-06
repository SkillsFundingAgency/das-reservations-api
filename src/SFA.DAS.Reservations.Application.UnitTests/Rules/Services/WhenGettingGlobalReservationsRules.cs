using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using GlobalRule = SFA.DAS.Reservations.Domain.Entities.GlobalRule;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenGettingGlobalReservationsRules
    {
        private Mock<IGlobalRuleRepository> _repository;
        private GlobalRulesService _globalRulesService;
        private GlobalRule _globalRule;

        [SetUp]
        public void Arrange()
        {
            _globalRule = new GlobalRule
            {
                Id = 3,
                Restriction = 0,
                RuleType = 1,
                ActiveFrom = new DateTime(2010,02,15)
            };

            _repository = new Mock<IGlobalRuleRepository>();
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<GlobalRule>{_globalRule});

            _repository.Setup(x => x.GetAll())
                .ReturnsAsync(new List<GlobalRule>{_globalRule});

            _globalRulesService = new GlobalRulesService(_repository.Object, 
                Mock.Of<IOptions<ReservationsConfiguration>>(), 
                Mock.Of<IAccountReservationService>(),
                Mock.Of<IAccountsService>(),
                Mock.Of<ILogger<GlobalRulesService>>());
        }

        [Test]
        public async Task Then_The_Global_Rules_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _globalRulesService.GetAllRules();

            //Assert
            _repository.Verify(x => x.GetAll());
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Then_The_Rule_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _globalRulesService.GetAllRules();

            //Assert
            Assert.IsAssignableFrom<List<Domain.Rules.GlobalRule>>(actual);
            Assert.IsNotEmpty(actual);
            var actualRule = actual.FirstOrDefault();
            Assert.IsNotNull(actualRule);
            Assert.AreEqual(_globalRule.Id, actualRule.Id);
            Assert.AreEqual(_globalRule.ActiveFrom, actualRule.ActiveFrom);
            Assert.AreEqual(_globalRule.ActiveTo, actualRule.ActiveTo);
            Assert.AreEqual(_globalRule.RuleType, (byte)actualRule.RuleType);
            Assert.AreEqual(_globalRule.Restriction, (byte)actualRule.Restriction);
        }
    }
}
