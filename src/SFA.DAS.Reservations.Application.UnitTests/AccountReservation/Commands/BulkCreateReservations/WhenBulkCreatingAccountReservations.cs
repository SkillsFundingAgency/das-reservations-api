using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Reservations.Domain.Validation.ValidationResult;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.BulkCreateReservations
{
    public class WhenBulkCreatingAccountReservations
    {
        [Test, MoqAutoData]
        public async Task ThenWillCreateMultipleReservations(
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] BulkCreateAccountReservationsCommandValidator validator,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService)
        {
            //Arrange
            var handler = new BulkCreateAccountReservationsCommandHandler(mockAccountReservationService.Object, validator);

            //Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            mockAccountReservationService.Verify(s => s.BulkCreateAccountReservation(command.ReservationCount), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task ThenWillValidateTheCommand(
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] Mock<IValidator<BulkCreateAccountReservationsCommand>> mockValidator,
            [Frozen] IAccountReservationService mockAccountReservationService)
        {
            //Arrange
            mockValidator.Setup(x => x.ValidateAsync(It.IsAny<BulkCreateAccountReservationsCommand>()))
                .ReturnsAsync(() => new ValidationResult());

            var handler = new BulkCreateAccountReservationsCommandHandler(mockAccountReservationService, mockValidator.Object);

            //Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            mockValidator.Verify(s => s.ValidateAsync(command), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task ThenThrowExceptionIfValidationFails(
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] Mock<IValidator<BulkCreateAccountReservationsCommand>> mockValidator,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService)
        {
            //Arrange
            mockValidator.Setup(x => x.ValidateAsync(It.IsAny<BulkCreateAccountReservationsCommand>())).ReturnsAsync(
                () => new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        {"Error", "Test Error"}
                    }
                });

            var handler = new BulkCreateAccountReservationsCommandHandler(mockAccountReservationService.Object, mockValidator.Object);

            //Act
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

            //Assert
            mockAccountReservationService.Verify(s => s.BulkCreateAccountReservation(It.IsAny<uint>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task ThenWillReturnCreatedReservationIds(
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] BulkCreateAccountReservationsCommandValidator validator,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService)
        {
            //Arrange
            var createdReservationIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            var handler = new BulkCreateAccountReservationsCommandHandler(mockAccountReservationService.Object, validator);
            mockAccountReservationService.Setup(s => s.BulkCreateAccountReservation(It.IsAny<uint>()))
                .ReturnsAsync(createdReservationIds);

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.AreEqual(createdReservationIds, result.CreatedReservationIds);
        }

        [Test, MoqAutoData]
        public void ThenWillThrowServiceExceptions(
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] BulkCreateAccountReservationsCommandValidator validator,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService)
        {
            //Arrange
            var expectedException = new Exception();

            var handler = new BulkCreateAccountReservationsCommandHandler(mockAccountReservationService.Object, validator);
            mockAccountReservationService.Setup(s => s.BulkCreateAccountReservation(It.IsAny<uint>()))
                .Throws(expectedException);

            //Act 
            var actualException = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

            //Assert
            Assert.AreEqual(expectedException, actualException);
        }
    }
}
