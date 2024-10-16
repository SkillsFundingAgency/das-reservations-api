using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using GlobalRule = SFA.DAS.Reservations.Domain.Entities.GlobalRule;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenGettingActiveGlobalReservationsRules
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
        public async Task Then_The_Active_Global_Rules_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _globalRulesService.GetActiveRules(DateTime.UtcNow);

            //Assert
            _repository.Verify(x => x.FindActive(It.Is<DateTime>(c => c.ToShortDateString().Equals(DateTime.UtcNow.ToShortDateString()))));
            actual.Should().NotBeNull();
        }

        [Test]
        public async Task Then_The_Active_Rule_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _globalRulesService.GetActiveRules(DateTime.UtcNow);

            //Assert
            var actualList = actual.Should().BeAssignableTo<List<Domain.Rules.GlobalRule>>().Subject;
            actualList.Should().NotBeEmpty();

            var actualRule = actualList.FirstOrDefault();
            actualRule.Should().NotBeNull();
            actualRule.Id.Should().Be(_globalRule.Id);
            actualRule.ActiveFrom.Should().Be(_globalRule.ActiveFrom);
            actualRule.RuleType.Should().Be((GlobalRuleType)_globalRule.RuleType);
            actualRule.Restriction.Should().Be((AccountRestriction)_globalRule.Restriction);
        }
    }
}
