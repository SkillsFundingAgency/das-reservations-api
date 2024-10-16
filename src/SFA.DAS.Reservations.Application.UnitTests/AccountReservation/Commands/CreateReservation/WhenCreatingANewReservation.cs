using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Infrastructure.Configuration;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.UnitOfWork.Context;
using AccountLegalEntity = SFA.DAS.Reservations.Domain.AccountLegalEntities.AccountLegalEntity;
using GlobalRule = SFA.DAS.Reservations.Domain.Rules.GlobalRule;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.CreateReservation
{
    public class WhenCreatingANewReservation
    {
        private CreateAccountReservationCommand _command;
        private CreateAccountReservationCommandHandler _handler;
        private const long ExpectedAccountId = 43532;
        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private readonly DateTime _expectedStartDate = DateTime.UtcNow;
        private readonly DateTime _expectedExpiryDate = DateTime.UtcNow;
        private const uint ExpectedProviderId = 1333333337;
        private CancellationToken _cancellationToken;
        private Mock<IAccountReservationService> _accountReservationsService;
        private Mock<IValidator<CreateAccountReservationCommand>> _validator;
        private Reservation _reservationCreated;
        private Mock<IGlobalRulesService> _globalRulesService;
        private Mock<IUnitOfWorkContext> _unitOfWork;
        private Mock<IAccountLegalEntitiesService> _accountLegalEntitiesService;
        private AccountLegalEntity _expectedAccountLegalEntity;
        private CurrentDateTime _currentDateTime;

        [SetUp]
        public void Arrange()
        {
            _cancellationToken = new CancellationToken();
              
            _reservationCreated = new Reservation(null, _expectedReservationId, ExpectedAccountId,false,DateTime.UtcNow, 
                DateTime.UtcNow.AddMonths(1), DateTime.UtcNow.AddMonths(3), ReservationStatus.Pending,new Course
            {
                CourseId = "1",
                Level = 1,
                Title = "Test Course"
            },ExpectedProviderId,198,"TestName",0, null );

            _validator = new Mock<IValidator<CreateAccountReservationCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateAccountReservationCommand>()))
                .ReturnsAsync(new ValidationResult{ValidationDictionary = new Dictionary<string, string>{{"",""}}});
            _validator.Setup(x=>x.ValidateAsync(
                    It.Is<CreateAccountReservationCommand>(c=>c.Id.Equals(_expectedReservationId))))
                .ReturnsAsync(new ValidationResult());

            _globalRulesService = new Mock<IGlobalRulesService>();
            _globalRulesService.Setup(x => x.GetActiveRules(It.IsAny<DateTime>()))
                               .ReturnsAsync(new List<GlobalRule>());

            _command = new CreateAccountReservationCommand
            {
                Id = _expectedReservationId, 
                AccountId = ExpectedAccountId, 
                AccountLegalEntityId = 198,
                AccountLegalEntityName = "TestName",
                StartDate = _expectedStartDate,
                CreatedDate = _expectedExpiryDate,
                ProviderId = ExpectedProviderId
            };

            _accountReservationsService = new Mock<IAccountReservationService>();
            _accountReservationsService
                .Setup(x => x.CreateAccountReservation(_command))
                .ReturnsAsync(_reservationCreated);

            _accountLegalEntitiesService = new Mock<IAccountLegalEntitiesService>();
            _expectedAccountLegalEntity = new AccountLegalEntity(Guid.NewGuid(), _command.AccountId, "Test Name 2", 1, _command.AccountLegalEntityId,true,true);
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(_command.AccountLegalEntityId))
                .ReturnsAsync(_expectedAccountLegalEntity);

            _unitOfWork = new Mock<IUnitOfWorkContext>();
            _currentDateTime = new CurrentDateTime();

            _handler = new CreateAccountReservationCommandHandler(_accountReservationsService.Object, _validator.Object, _globalRulesService.Object, _unitOfWork.Object, _accountLegalEntitiesService.Object, _currentDateTime);
        }

        [Test]
        public void Then_The_Command_Is_Validated_And_The_Service_Not_Called_If_Not_Valid()
        {
            //Arrange
            var expectedCommand = new CreateAccountReservationCommand {AccountId = 1};

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _handler.Handle(expectedCommand, _cancellationToken);
            });
            _accountReservationsService.Verify(x => 
                x.CreateAccountReservation(It.IsAny<CreateAccountReservationCommand>()), Times.Never);
            _unitOfWork.Verify(x => x.AddEvent(It.IsAny<ReservationCreatedEvent>()), Times.Never);
        }

        [Test]
        public async Task Then_The_Request_Is_Sent_To_The_Service_If_Valid()
        {
            //Act
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountReservationsService.Verify(x=>x.CreateAccountReservation(_command),Times.Once);
        }

        [Test]
        public async Task Then_The_Request_Is_Validated_Against_The_Global_Rules()
        {
            //Act 
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _globalRulesService.Verify(x=>x.CheckReservationAgainstRules(_command),Times.Once);
        }

        [Test]
        public async Task Then_If_There_Are_Global_Restrictions_In_Place_The_Reservation_Is_Not_Created_And_Global_Rules_Returned()
        {
            //Arrange
            _globalRulesService.Setup(x => x.CheckReservationAgainstRules(_command))
                .ReturnsAsync(new GlobalRule(new Domain.Entities.GlobalRule {Id = 1, Restriction = 1, RuleType = 1}));

            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountReservationsService.Verify(x => x.CreateAccountReservation(_command), Times.Never);
            actual.Rule.Should().NotBeNull();
        }

        [Test]
        public async Task Then_If_There_Are_Global_Restrictions_In_Place_For_Non_Levy_Then_The_Levy_Reservation_Is_Created_And_Global_Rules_Returned()
        {
            //Arrange
            _command.IsLevyAccount = true;
            _globalRulesService.Setup(x => x.CheckReservationAgainstRules(_command))
                .ReturnsAsync(new GlobalRule(new Domain.Entities.GlobalRule { Id = 1, Restriction = (byte)AccountRestriction.NonLevy, RuleType = 1 }));

            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountReservationsService.Verify(x => x.CreateAccountReservation(_command), Times.Once);
            actual.Rule.Should().BeNull();
        }

        [Test]
        public async Task Then_If_The_Request_Is_For_A_NonLevy_Agreement_That_Is_Signed_Then_The_Reservation_Is_Created()
        {
            //Arrange
            _expectedAccountLegalEntity = new AccountLegalEntity(Guid.NewGuid(), _command.AccountId, "Test Name 2", 1,
                _command.AccountLegalEntityId, true, false);
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(_command.AccountLegalEntityId))
                .ReturnsAsync(_expectedAccountLegalEntity);

            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountReservationsService.Verify(x => x.CreateAccountReservation(_command), Times.Once);
        }

        [Test]
        public async Task Then_If_The_Request_Is_For_A_NonLevy_Agreement_That_Is_Not_Signed_Then_The_Reservation_Is_Not_Created()
        {
            //Arrange
            _expectedAccountLegalEntity = new AccountLegalEntity(Guid.NewGuid(), _command.AccountId, "Test Name 2", 1,
                _command.AccountLegalEntityId, false, false);
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(_command.AccountLegalEntityId))
                .ReturnsAsync(_expectedAccountLegalEntity);

            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountReservationsService.Verify(x => x.CreateAccountReservation(_command), Times.Never);
        }

        [Test]
        public async Task Then_A_ReservationCreated_Message_Is_Added_When_Successful()
        {
            //Act
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _unitOfWork.Verify(x=>x.AddEvent(It.Is<Func<ReservationCreatedEvent>>(c => 
                c.Invoke().Id.Equals(_command.Id)
                && c.Invoke().AccountId.Equals(_reservationCreated.AccountId)
                && c.Invoke().AccountLegalEntityId.Equals(_command.AccountLegalEntityId)
                && c.Invoke().AccountLegalEntityName.Equals(_command.AccountLegalEntityName)
                && c.Invoke().ProviderId.Equals(_reservationCreated.ProviderId)
                && c.Invoke().CourseId.Equals(_reservationCreated.Course.CourseId)
                && c.Invoke().CourseName.Equals(_reservationCreated.Course.Title)
                && c.Invoke().CourseLevel.Equals(_reservationCreated.Course.Level)
                && c.Invoke().StartDate.Equals(_reservationCreated.StartDate)
                && c.Invoke().EndDate.Equals(_reservationCreated.ExpiryDate)
                && c.Invoke().CreatedDate.Equals(_reservationCreated.CreatedDate)
            )),Times.Once);
        }

        [Test]
        public async Task Then_If_The_Reservation_Is_For_Levy_The_Account_Name_Is_Populated()
        {
            //Arrange
            _command.IsLevyAccount = true;

            //Act
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountLegalEntitiesService.Verify(x=>x.GetAccountLegalEntity(_command.AccountLegalEntityId), Times.Once);
            _accountReservationsService.Verify(x => x.CreateAccountReservation(It.Is<CreateAccountReservationCommand>(c=>c.AccountLegalEntityName.Equals(_expectedAccountLegalEntity.AccountLegalEntityName))), Times.Once);
        }

        [Test]
        public async Task Then_If_The_Reservation_Is_For_Levy_The_Event_Is_Created_With_Min_Dates()
        {
            //Arrange
            _command.IsLevyAccount = true;
            _command.StartDate = null;
            _reservationCreated = new Reservation(null, _expectedReservationId, ExpectedAccountId, true, DateTime.UtcNow,
                null, null, ReservationStatus.Pending, new Course
                {
                    CourseId = "1",
                    Level = 1,
                    Title = "Test Course"
                }, null, 198, "TestName",0, null);
            _accountReservationsService
                .Setup(x => x.CreateAccountReservation(_command))
                .ReturnsAsync(_reservationCreated);
            _handler = new CreateAccountReservationCommandHandler(_accountReservationsService.Object, _validator.Object, _globalRulesService.Object, _unitOfWork.Object, _accountLegalEntitiesService.Object, _currentDateTime);

            //Act
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _unitOfWork.Verify(x => x.AddEvent(It.Is<Func<ReservationCreatedEvent>>(c =>
                c.Invoke().Id.Equals(_expectedReservationId)
                && c.Invoke().AccountLegalEntityId.Equals(_reservationCreated.AccountLegalEntityId)
                && c.Invoke().AccountLegalEntityName.Equals(_reservationCreated.AccountLegalEntityName)
                && c.Invoke().StartDate.Equals(DateTime.MinValue)
                && c.Invoke().EndDate.Equals(DateTime.MinValue)
                && c.Invoke().CreatedDate.Equals(_reservationCreated.CreatedDate)
                && c.Invoke().AccountId.Equals(_reservationCreated.AccountId)
            )), Times.Once);
        }

        [Test]
        public async Task Then_The_Reservation_Is_Returned_In_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            actual.Reservation.Should().Be(_reservationCreated);
            actual.AgreementSigned.Should().BeTrue();
        }

        [Test]
        public async Task Then_If_The_Command_Is_For_A_NonLevy_Reservation_The_Start_Date_Must_Be_Present()
        {
            var expectedErrorMessage = "You must enter a start date to reserve new funding";
            _command.StartDate = null;

            var act = async () => await _handler.Handle(_command, _cancellationToken);

            //Assert
            act.Should().ThrowAsync<StartDateException>().WithMessage(expectedErrorMessage);
        }

        [Test]
        public async Task Then_If_The_Command_Is_For_A_NonLevy_Reservation_The_Start_Date_Must_Not_Be_MinDate()
        {
            var expectedErrorMessage = "You must enter a start date to reserve new funding";
            _command.StartDate = DateTime.MinValue;

            var act = async () => await _handler.Handle(_command, _cancellationToken);

            //Assert
            act.Should().ThrowAsync<StartDateException>().WithMessage(expectedErrorMessage);
        }

        [TestCase(-2)]
        [TestCase(3)]
        public async Task Then_If_The_Command_Is_For_A_NonLevy_Reservation_The_Start_Date_Must_Not_Be_Outside_This_Range(int monthsToAdd)
        {
            var validFromDate = _currentDateTime.GetDate().AddMonths(-1).ToString("MM yyyy");
            var validToDate = _currentDateTime.GetDate().AddMonths(2).ToString("MM yyyy");
            var expectedErrorMessage = $"Training start date must be between the funding reservation dates {validFromDate} to {validToDate}";

            var startDate = _currentDateTime.GetDate().AddMonths(monthsToAdd);
            var firstOfMonthStartDate = new DateTime(startDate.Year, startDate.Month, 1);
            _command.StartDate = firstOfMonthStartDate;

            var act = async () => await _handler.Handle(_command, _cancellationToken);
                
            //Assert
            act.Should().ThrowAsync<StartDateException>().WithMessage(expectedErrorMessage);
        }

        [TestCase(-1)]
        [TestCase(2)]
        public async Task Then_If_The_Command_Is_For_A_NonLevy_Reservation_The_Start_Date_Must_Be_Within_This_Range(int monthsToAdd)
        {
            var startDate = _currentDateTime.GetDate().AddMonths(monthsToAdd);
            var firstOfMonthStartDate = new DateTime(startDate.Year, startDate.Month, 1);
            _command.StartDate = firstOfMonthStartDate;

            var result = await _handler.Handle(_command, _cancellationToken);

            result.Reservation.Should().NotBeNull();
        }
    }
}
