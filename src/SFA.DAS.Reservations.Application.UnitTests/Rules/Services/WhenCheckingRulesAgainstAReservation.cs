using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
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
       
        private Mock<IOptions<ReservationsConfiguration>> _options;
        private Mock<IAccountReservationService> _reservationRepository;
        private Mock<IAccountsService> _accountsService;
        private ReservationsConfiguration options;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IGlobalRuleRepository>();
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<GlobalRule>
                {
                    new GlobalRule
                    {
                        ActiveFrom = DateTime.UtcNow.AddDays(-1),
                        Restriction = 1,
                        RuleType = 0,
                        Id = 123
                    }
                });
            _reservationRepository = new Mock<IAccountReservationService>();
            _reservationRepository.Setup(x => x.GetAccountReservations(It.IsAny<long>()))
                .ReturnsAsync(new List<Reservation>());

             options = new ReservationsConfiguration { ResetReservationDate = DateTime.MinValue, ExpiryPeriodInMonths = 1 };
            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value).Returns(() => options);

            _accountsService = new Mock<IAccountsService>();
            _accountsService.Setup(x => x.GetAccount(It.IsAny<long>()))
                .ReturnsAsync(new Domain.Account.Account(1,false,"",4));

            _globalRulesService = new GlobalRulesService(_repository.Object, _options.Object, _reservationRepository.Object, _accountsService.Object, Mock.Of<ILogger<GlobalRulesService>>());
        }

        [Test]
        public async Task Then_If_There_Is_A_Rule_Matching_The_Reservation_It_Is_Returned()
        {
            //Arrange
            var reservation = new Reservation(Guid.NewGuid(), 123, DateTime.UtcNow, 2, "test");

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNotNull(actual);
        }


        [Test]
        public async Task Then_If_There_Is_A_Rule_Matching_The_Levy_Reservation_It_Is_Returned()
        {
            //Arrange
            var reservation = new Reservation(Guid.NewGuid(), 123, null, 2,"Test");

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
                StartDate = DateTime.UtcNow,
                CourseId = "13",
                IsLevyAccount = true,
                CreatedDate = DateTime.UtcNow
            };

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_There_Are_No_Matching_Rules_Null_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<GlobalRule>());

            var reservation = new Reservation(Guid.NewGuid(), 123, DateTime.UtcNow, 2, "test");

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_Multiple_Rules_Are_Checked_And_First_Returned_That_Matches()
        {
            //Arrange
            var reservation = new Reservation(Guid.NewGuid(), 123, DateTime.UtcNow, 2, "test");
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<GlobalRule>
                {
                    new GlobalRule
                    {
                        ActiveFrom = DateTime.UtcNow.AddDays(-1),
                        Restriction = 1,
                        RuleType = 1,
                        Id = 123
                    },
                    new GlobalRule
                    {
                        ActiveFrom = DateTime.UtcNow.AddDays(-1),
                        Restriction = 1,
                        RuleType = 1,
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
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test");

            //Act
            await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            _accountsService.Verify(x => x.GetAccount(expectedAccountId), Times.Once);
        }

        [Test]
        public async Task Then_If_The_Max_Number_Of_Non_Levy_Reservations_Has_Been_Met_Then_The_Rule_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test");
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(1);
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Reservation>
            {
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", isLevyAccount: true), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name"), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name")
            });

            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, false, "test", 2));

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(AccountRestriction.Account,actual.Restriction);
            Assert.AreEqual(GlobalRuleType.ReservationLimit, actual.RuleType);
        }

        [Test]
        public async Task Then_If_The_Max_Number_Of_Non_Levy_Reservations_Has_Been_Met_Before_Reservation_Reset_Date_Then_The_Rule_Is_Not_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test");
            options.MaxNumberOfReservations = 1;
            options.ResetReservationDate = DateTime.UtcNow;

            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Reservation>
            {new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", isLevyAccount: true, createdDate: DateTime.UtcNow.AddMonths(-1)),
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", createdDate: DateTime.UtcNow.AddMonths(-1)),
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", createdDate: DateTime.UtcNow.AddMonths(-1))
            }); ;

            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, false, "test", 2));

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_The_Max_Number_Of_Non_Levy_Reservations_Has_Been_Met_After_Reservation_Reset_Date_Then_The_Rule_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test");
            options.MaxNumberOfReservations = 1;
            options.ResetReservationDate = DateTime.UtcNow;

            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Reservation>
            {new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", isLevyAccount: true, createdDate: DateTime.UtcNow.AddMonths(-1)),
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", createdDate: DateTime.UtcNow.AddMonths(1)),
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", createdDate: DateTime.UtcNow.AddMonths(1))
            }); ;

            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, false, "test", 2));

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(AccountRestriction.Account, actual.Restriction);
            Assert.AreEqual(GlobalRuleType.ReservationLimit, actual.RuleType);
        }

        [Test]
        public async Task Then_If_The_Max_Number_Of_Non_Levy_Reservations_Has_Not_Been_Met_Then_Null_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test");
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(2);
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Reservation>
            {
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name"), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", isLevyAccount: true), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", isLevyAccount: true)
            });

            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, false, "test", 2));

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_No_Maximum_Has_Been_Set_It_Returns_Null()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns((int?)null);

            var expectedAccountId = 123;

            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, false, "test", null));
            var existingReservations = new List<Reservation>();
            for (var i = 0; i<5;i++)
            {
                existingReservations.Add(new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name"));
            }
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId))
                .ReturnsAsync(existingReservations);
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test");

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
            
        }

        [Test]
        public async Task Then_If_The_Max_Number_Of_Non_Levy_Reservations_Has_Been_MetAnd_The_Account_is_Levy_Then_A_Null_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test", isLevyAccount:true);
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(1);
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Reservation>
            {
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", isLevyAccount: true), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name"), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name")
            });

            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, true, "test", 2));

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_The_Max_Number_Of_Reservations_Has_Been_Reached_And_A_Levy_Reservation_Is_Created_Then_Null_Is_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var reservation = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 2, "test", isLevyAccount: true);
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(1);
            _reservationRepository.Setup(x => x.GetAccountReservations(expectedAccountId)).ReturnsAsync(new List<Reservation>
            {
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name", isLevyAccount: true), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name"), 
                new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow, 3, "Name")
            });
            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, true, "test", 2));

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task Then_If_It_Is_A_levy_Reservation_The_Existing_Reservations_Are_Not_Checked_To_Determine_Whether_Limit_Has_Been_Reached()
        {
            //Arrange
            var reservation = new Reservation(Guid.NewGuid(), 123, DateTime.UtcNow, 2, "test", isLevyAccount: true);
            
            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(reservation);
            
            //Assert
            _accountsService.Verify(x => x.GetAccount(It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task And_Reservation_Limit_Reached_And_Reservation_Expired_Then_Null_Returned()
        {
            //Arrange
            _repository.Setup(x => x.FindActive(It.IsAny<DateTime>())).ReturnsAsync(new List<GlobalRule>());
            var expectedAccountId = 123;
            var request = new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow.Date, 2, "test");
            _reservationRepository
                .Setup(x => x.GetAccountReservations(expectedAccountId))
                .ReturnsAsync(new List<Reservation>
                {
                    new Reservation(Guid.NewGuid(), expectedAccountId, DateTime.UtcNow.Date.AddMonths(-3), 2, "Name")
                });

            _accountsService.Setup(x => x.GetAccount(expectedAccountId)).ReturnsAsync(
                new Domain.Account.Account(1, false, "test", 1));

            //Act
            var actual = await _globalRulesService.CheckReservationAgainstRules(request);

            //Assert
            Assert.IsNull(actual);
        }
    }
}
