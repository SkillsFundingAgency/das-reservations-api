﻿using System;
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

        [SetUp]
        public void Arrange()
        {
            _cancellationToken = new CancellationToken();
            _reservation = new Reservation(Mock.Of<IRuleRepository>())
            {
                AccountId = ExpectedAccountId,
                StartDate = _expectedDateTime
            };

            _validator = new Mock<IValidator<CreateAccountReservationCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateAccountReservationCommand>()))
                .ReturnsAsync(new ValidationResult{ValidationDictionary = new Dictionary<string, string>{{"",""}}});
            _validator.Setup(x=>x.ValidateAsync(It.Is<CreateAccountReservationCommand>(c=>c.Reservation.AccountId.Equals(ExpectedAccountId))))
                .ReturnsAsync(new ValidationResult());

            _accountReservationsService = new Mock<IAccountReservationService>();
            _accountReservationsService.Setup(x => x.CreateAccountReservation(It.Is<Reservation>(c=>c.AccountId.Equals(ExpectedAccountId) && c.StartDate.Equals(_expectedDateTime))))
                .ReturnsAsync(ExpectedReservationId);

            

            _command = new CreateAccountReservationCommand {Reservation = _reservation};

            _handler = new CreateAccountReservationCommandHandler(_accountReservationsService.Object, _validator.Object);
        }

        [Test]
        public void Then_The_Command_Is_Validated_And_The_Service_Not_Called_If_Not_Valid()
        {
            //Arrange
            var expectedCommand = new CreateAccountReservationCommand {Reservation = new Reservation(Mock.Of<IRuleRepository>()){AccountId = 1 }};

            //Act Assert
            Assert.ThrowsAsync<InvalidOperationException>(async() =>
            {
                await _handler.Handle(expectedCommand, _cancellationToken);
            });
            _accountReservationsService.Verify(x => x.CreateAccountReservation(It.IsAny<Reservation>()), Times.Never);
        }

        [Test]
        public async Task Then_If_The_Command_IsValid_Then_CreateReservation_Is_Called_On_The_Service()
        {
            //Act
            await _handler.Handle(_command, _cancellationToken);

            //Assert
            _accountReservationsService.Verify(x=>x.CreateAccountReservation(
                It.Is<Reservation>(
                    c=>
                        c.AccountId.Equals(_command.Reservation.AccountId) 
                       && c.StartDate.Equals(_expectedDateTime)
                        )), Times.Once);
        }

        [Test]
        public async Task Then_The_ReservationId_Is_Returned_In_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_command, _cancellationToken);

            //Assert
            Assert.AreEqual(ExpectedReservationId, actual.ReservationId);
        }
    }
}