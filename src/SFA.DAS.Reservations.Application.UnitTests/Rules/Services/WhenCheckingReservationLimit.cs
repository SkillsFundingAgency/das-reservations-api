using System;
using System.Collections.Generic;
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

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenCheckingReservationLimit
    {
        private Mock<IAccountReservationService> _repository;
        private Mock<IOptions<ReservationsConfiguration>> _options;
        private GlobalRulesService _globalRulesService;
        private Domain.Entities.GlobalRule _globalRule;
        private Mock<IAccountsService> _accountService;
        private const long ExpectedAccountId = 534542143;
        private const int ReservationLimit = 2;

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
                .ReturnsAsync(new Domain.Account.Account(ExpectedAccountId, false, "test", ReservationLimit));

            ReservationsConfiguration options = new ReservationsConfiguration { ResetReservationDate = DateTime.MinValue };
            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value).Returns(options);

            _globalRulesService = new GlobalRulesService(Mock.Of<IGlobalRuleRepository>(), _options.Object, _repository.Object, _accountService.Object, Mock.Of<ILogger<GlobalRulesService>>());
        }

        [TestCase(true, false)]
        [TestCase(false, false)]
        public async Task And_Reservation_Limit_Not_Exceeded_Then_Return_Expected(bool isLevyAccount, bool expected)
        {
            // Arrange
            _repository.Setup(x => x.GetRemainingReservations(ExpectedAccountId, ReservationLimit)).ReturnsAsync(1);

            //Act
            var actual = await _globalRulesService.HasReachedReservationLimit(ExpectedAccountId, isLevyAccount);

            // Assert
            actual.Should().Be(expected);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task And_Reservation_Limit_Exceeded_Then_Return_Expected(bool isLevyAccount, bool expected)
        {
            //Act
            var actual = await _globalRulesService.HasReachedReservationLimit(ExpectedAccountId, isLevyAccount);

            //Assert
            actual.Should().Be(expected);
        }
    }
}
