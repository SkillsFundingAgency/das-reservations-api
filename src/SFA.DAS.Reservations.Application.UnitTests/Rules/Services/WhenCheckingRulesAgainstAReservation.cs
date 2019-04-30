using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using GlobalRule = SFA.DAS.Reservations.Domain.Entities.GlobalRule;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenCheckingRulesAgainstAReservation
    {
        private Mock<IGlobalRuleRepository> _repository;
        private GlobalRulesService _globalRulesService;

        private readonly DateTime _dateFrom = new DateTime(2019, 02, 10);
        private Mock<IOptions<ReservationsConfiguration>> _options;
        private Mock<IReservationRepository> _reservationRepository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IGlobalRuleRepository>();
            _repository.Setup(x => x.GetGlobalRules(_dateFrom))
                .ReturnsAsync(new List<GlobalRule>
                {
                    new GlobalRule
                    {
                        ActiveFrom = _dateFrom.AddDays(-1),
                        Restriction = 1,
                        RuleType = 0,
                        Id = 123
                    }
                });
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationRepository.Setup(x => x.GetAccountReservations(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Reservation>());

            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value.ExpiryPeriodInMonths).Returns(1);
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(3);

            _globalRulesService = new GlobalRulesService(_repository.Object, _options.Object, _reservationRepository.Object);
        }

        [Test]
        public async Task Then_If_There_Is_A_Rule_Matching_The_Reservation_It_Is_Returned()
        {
            //Arrange
            var reservation = new Reservation(Guid.NewGuid(), 123, _dateFrom, 2, "test");

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Then_The_Restriction_Type_Is_Checked_Against_The_Reservation()
        {
            //Arrange
            var reservation = new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                AccountLegalEntityId = 123,
                AccountLegalEntityName = "Test",
                StartDate = _dateFrom,
                CourseId = "13",
                IsLevyAccount = true
            };

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_There_Are_No_Matching_Rules_Null_Is_Returned()
        {
            var reservation = new Reservation(Guid.NewGuid(), 123, _dateFrom.AddDays(-3), 2, "test");

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_Multiple_Rules_Are_Checked_And_First_Returned_That_Matches()
        {
            //Arrange
            var reservation = new Reservation(Guid.NewGuid(), 123, _dateFrom, 2, "test");
            _repository.Setup(x => x.GetGlobalRules(_dateFrom))
                .ReturnsAsync(new List<GlobalRule>
                {
                    new GlobalRule
                    {
                        ActiveFrom = _dateFrom.AddDays(-1),
                        Restriction = 1,
                        RuleType = 0,
                        Id = 123
                    },
                    new GlobalRule
                    {
                        ActiveFrom = _dateFrom.AddDays(-1),
                        Restriction = 0,
                        RuleType = 0,
                        Id = 123
                    }
                });
            
            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Then_The_Number_Of_Reservations_Limit_Is_Checked_For_The_Request()
        {
            //Arrange
            var expectedAccountId = 123;
            _repository.Setup(x => x.GetGlobalRules(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, _dateFrom, 2, "test");

            //Act
            await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            _reservationRepository.Verify(x => x.GetAccountReservations(expectedAccountId), Times.Once);
        }

        [Test]
        public async Task Then_If_The_Max_Number_Of_Reservations_Has_Been_Met_Then_The_Rule_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.GetGlobalRules(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, _dateFrom, 2, "test");
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(1);
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Domain.Entities.Reservation>{new Domain.Entities.Reservation()});

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(AccountRestriction.Account,actual.Restriction);
            Assert.AreEqual(GlobalRuleType.ReservationLimit, actual.RuleType);
        }


        [Test]
        public async Task Then_If_The_Max_Number_Of_Reservations_Has_Not_Been_Met_Then_Null_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.GetGlobalRules(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, _dateFrom, 2, "test");
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(2);
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Domain.Entities.Reservation> { new Domain.Entities.Reservation() });

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [TestCase(1,true)]
        [TestCase(2,true)]
        [TestCase(3,false)]
        [TestCase(4,false)]
        public async Task Then_If_No_Maximum_Has_Been_Set_It_Defaults_To_Three_Reservations(int numberOfReservations, bool isNull)
        {
            //Arrange
            _repository.Setup(x => x.GetGlobalRules(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(0);
            var expectedAccountId = 123;
            var existingReservations = new List<Domain.Entities.Reservation>();
            for (var i = 0; i<numberOfReservations;i++)
            {
                existingReservations.Add(new Domain.Entities.Reservation());
            }
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId))
                .ReturnsAsync(existingReservations);
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, _dateFrom, 2, "test");

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            if (isNull)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
            }
        }
    }
}