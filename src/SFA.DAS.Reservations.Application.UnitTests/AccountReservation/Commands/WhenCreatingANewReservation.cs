using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;
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

        [SetUp]
        public void Arrange()
        {
            _cancellationToken = new CancellationToken();
              
            _reservationCreated = new Reservation(_expectedReservationId,ExpectedAccountId,DateTime.UtcNow, 1, "TestName");
            
            _validator = new Mock<IValidator<CreateAccountReservationCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateAccountReservationCommand>()))
                .ReturnsAsync(new ValidationResult{ValidationDictionary = new Dictionary<string, string>{{"",""}}});
            _validator.Setup(x=>x.ValidateAsync(It.Is<CreateAccountReservationCommand>(c=>c.Id.Equals(_expectedReservationId))))
                .ReturnsAsync(new ValidationResult());

            _command = new CreateAccountReservationCommand {Id = _expectedReservationId, AccountId = ExpectedAccountId, StartDate = _expectedDateTime};

            _accountReservationsService = new Mock<IAccountReservationService>();
            _accountReservationsService
                .Setup(x => x.CreateAccountReservation(_command))
                .ReturnsAsync(_reservationCreated);


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
            _accountReservationsService.Verify(x => x.CreateAccountReservation(It.IsAny<CreateAccountReservationCommand>()), Times.Never);
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
        public async Task Then_The_Reservation_Is_Returned_In_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            Assert.AreEqual(_reservationCreated, actual.Reservation);
        }
    }
}
