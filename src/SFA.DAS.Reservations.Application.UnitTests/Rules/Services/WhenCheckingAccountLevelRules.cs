using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenCheckingAccountLevelRules
    {
        private Mock<IReservationRepository> _repository;
        private GlobalRulesService _globalRulesService;
        private Domain.Entities.GlobalRule _globalRule;
        private Mock<IOptions<ReservationsConfiguration>> _options;
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
            _repository = new Mock<IReservationRepository>();
            _repository.Setup(x => x.GetAccountReservations(ExpectedAccountId)).ReturnsAsync(new List<Reservation>{new Reservation()});
            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(1);

            _globalRulesService = new GlobalRulesService(Mock.Of<IGlobalRuleRepository>(), _options.Object , _repository.Object);
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
            Assert.IsAssignableFrom<List<Domain.Rules.GlobalRule>>(actual);
            Assert.IsNotEmpty(actual);
            var actualRule = actual.FirstOrDefault();
            Assert.IsNotNull(actualRule);
            Assert.AreEqual(_globalRule.Id, actualRule.Id);
            Assert.AreEqual(_globalRule.RuleType, (byte)actualRule.RuleType);
            Assert.AreEqual(_globalRule.Restriction, (byte)actualRule.Restriction);
        }
    }
}
