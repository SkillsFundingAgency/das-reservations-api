using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands
{
    public class WhenCreatingANewReservation
    {
        private CreateAccountReservationCommand _command;
        private CreateAccountReservationCommandHandler _handler;
        private const long ExpectedAccountId = 43532;
        private const long ExpectedReservationId = 44212;
        private readonly DateTime _expectedDateTime = DateTime.UtcNow;
        private CancellationToken _cancellationToken;
        private Mock<IAccountReservationService> _accountReservationsService;
        private Mock<IValidator<CreateAccountReservationCommand>> _validator;
        private Reservation _reservation;
        private Reservation _reservationCreated;

        [SetUp]
        public void Arrange()
        {
            _cancellationToken = new CancellationToken();
            _reservation = new Reservation(ExpectedAccountId, _expectedDateTime, 3);
              
            _reservationCreated = new Reservation(null,ExpectedReservationId,ExpectedAccountId,false,DateTime.UtcNow, DateTime.UtcNow,DateTime.UtcNow, ReservationStatus.Pending);
            
            _validator = new Mock<IValidator<CreateAccountReservationCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateAccountReservationCommand>()))
                .ReturnsAsync(new ValidationResult{ValidationDictionary = new Dictionary<string, string>{{"",""}}});
            _validator.Setup(x=>x.ValidateAsync(It.Is<CreateAccountReservationCommand>(c=>c.AccountId.Equals(ExpectedAccountId))))
                .ReturnsAsync(new ValidationResult());

            _accountReservationsService = new Mock<IAccountReservationService>();
            _accountReservationsService.Setup(x => x.CreateAccountReservation(ExpectedAccountId, _expectedDateTime)).ReturnsAsync(_reservationCreated);

            

            _command = new CreateAccountReservationCommand {AccountId = ExpectedAccountId, StartDate = _expectedDateTime};

            _handler = new CreateAccountReservationCommandHandler(_accountReservationsService.Object, _validator.Object);
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
            _accountReservationsService.Verify(x => x.CreateAccountReservation(It.IsAny<long>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public async Task Then_If_The_Command_IsValid_Then_CreateReservation_Is_Called_On_The_Service()
        {
            //Act
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountReservationsService.Verify(x=>x.CreateAccountReservation(_command.AccountId,_expectedDateTime),Times.Once());
        }

        [Test]
        public async Task Then_The_ReservationId_Is_Returned_In_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            Assert.AreEqual(_reservationCreated, actual.Reservation);
        }
    }
}
