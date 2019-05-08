using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.UnitOfWork;
using GlobalRule = SFA.DAS.Reservations.Domain.Rules.GlobalRule;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands
{
    public class WhenCreatingANewReservation
    {
        private CreateAccountReservationCommand _command;
        private CreateAccountReservationCommandHandler _handler;
        private const long ExpectedAccountId = 43532;
        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private readonly DateTime _expectedDateTime = DateTime.UtcNow;
        private CancellationToken _cancellationToken;
        private Mock<IAccountReservationService> _accountReservationsService;
        private Mock<IValidator<CreateAccountReservationCommand>> _validator;
        private Reservation _reservationCreated;
        private Mock<IGlobalRulesService> _globalRulesService;
        private Mock<IUnitOfWorkContext> _unitOfWork;

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
            },null,198,"TestName" );

            _validator = new Mock<IValidator<CreateAccountReservationCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateAccountReservationCommand>()))
                .ReturnsAsync(new ValidationResult{ValidationDictionary = new Dictionary<string, string>{{"",""}}});
            _validator.Setup(x=>x.ValidateAsync(It.Is<CreateAccountReservationCommand>(c=>c.Id.Equals(_expectedReservationId))))
                .ReturnsAsync(new ValidationResult());

            _globalRulesService = new Mock<IGlobalRulesService>();
            _globalRulesService.Setup(x => x.GetRules()).ReturnsAsync(new List<GlobalRule>());

            _command = new CreateAccountReservationCommand {Id = _expectedReservationId, AccountId = ExpectedAccountId, StartDate = _expectedDateTime,AccountLegalEntityId = 198,AccountLegalEntityName = "TestName"};

            _accountReservationsService = new Mock<IAccountReservationService>();
            _accountReservationsService
                .Setup(x => x.CreateAccountReservation(_command))
                .ReturnsAsync(_reservationCreated);

            _unitOfWork = new Mock<IUnitOfWorkContext>();

            _handler = new CreateAccountReservationCommandHandler(_accountReservationsService.Object, _validator.Object, _globalRulesService.Object, _unitOfWork.Object);
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
            _accountReservationsService.Verify(x => x.CreateAccountReservation(It.IsAny<CreateAccountReservationCommand>()), Times.Never);
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
            Assert.IsNotNull(actual.Rule);
        }

        [Test]
        public async Task Then_A_ReservationCreated_Message_Is_Added_When_Successful()
        {
            //Act
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _unitOfWork.Verify(x=>x.AddEvent(It.Is<Func<ReservationCreatedEvent>>(c => 
                c.Invoke().Id.Equals(_command.Id)
                && c.Invoke().AccountLegalEntityId.Equals(_command.AccountLegalEntityId)
                && c.Invoke().AccountLegalEntityName.Equals(_command.AccountLegalEntityName)
                && c.Invoke().CourseId.Equals(_reservationCreated.Course.CourseId)
                && c.Invoke().CourseName.Equals(_reservationCreated.Course.Title)
                && c.Invoke().StartDate.Equals(_reservationCreated.StartDate)
                && c.Invoke().EndDate.Equals(_reservationCreated.ExpiryDate)
                && c.Invoke().CreatedDate.Equals(_reservationCreated.CreatedDate)
                )),Times.Once);
        }

        [Test]
        public async Task Then_The_Reservation_Is_Returned_In_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            Assert.AreEqual(_reservationCreated, actual.Reservation);
        }
    }
}
