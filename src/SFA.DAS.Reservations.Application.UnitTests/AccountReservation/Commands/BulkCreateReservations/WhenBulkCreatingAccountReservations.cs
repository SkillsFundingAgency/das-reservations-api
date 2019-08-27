using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Reservations.Domain.Validation.ValidationResult;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.BulkCreateReservations
{
    public class WhenBulkCreatingAccountReservations
    {
        [Test, MoqAutoData]
        public void Then_Throws_An_Exception_If_ValidationFails(
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] Mock<IValidator<BulkCreateAccountReservationsCommand>> mockValidator,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService,
            BulkCreateAccountReservationsCommandHandler handler)
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

            //Act
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

            //Assert
            mockAccountReservationService.Verify(s => s.BulkCreateAccountReservation(It.IsAny<uint>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<long?>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Account_Is_Retrieved_From_The_AccountLegalEntityId_And_Passed_To_The_Service(
            BulkCreateAccountReservationsCommand command,
            AccountLegalEntity accountLegalEntity,
            [Frozen] Mock<IValidator<BulkCreateAccountReservationsCommand>> validator,
            [Frozen] Mock<IAccountLegalEntitiesService> accountLegalEntitiesService,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService,
            BulkCreateAccountReservationsCommandHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(It.IsAny<BulkCreateAccountReservationsCommand>()))
                .ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});
            accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(command.AccountLegalEntityId)).ReturnsAsync(accountLegalEntity);

            //Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            mockAccountReservationService.Verify(x=>x.BulkCreateAccountReservation(command.ReservationCount,command.AccountLegalEntityId, accountLegalEntity.AccountId, accountLegalEntity.AccountLegalEntityName, command.TransferSenderAccountId));
        }

        [Test, MoqAutoData]
        public async Task Then_Will_Return_Created_Reservation_Ids(
            List<Guid> createdReservationIds,
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] Mock<IValidator<BulkCreateAccountReservationsCommand>> validator,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService,
            BulkCreateAccountReservationsCommandHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(It.IsAny<BulkCreateAccountReservationsCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            mockAccountReservationService.Setup(s => s.BulkCreateAccountReservation(command.ReservationCount, command.AccountLegalEntityId, It.IsAny<long>(), It.IsAny<string>(), command.TransferSenderAccountId))
                .ReturnsAsync(createdReservationIds);

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.AreEqual(createdReservationIds, result.ReservationIds);
        }

        [Test, MoqAutoData]
        public void Then_Will_Throw_Service_Exceptions(
            [Frozen] BulkCreateAccountReservationsCommand command,
            [Frozen] Mock<IValidator<BulkCreateAccountReservationsCommand>> validator,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationService,
            BulkCreateAccountReservationsCommandHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(It.IsAny<BulkCreateAccountReservationsCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            var expectedException = new Exception();
            mockAccountReservationService.Setup(s => s.BulkCreateAccountReservation(It.IsAny<uint>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<long?>()))
                .Throws(expectedException);

            //Act 
            var actualException = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

            //Assert
            Assert.AreEqual(expectedException, actualException);
        }
    }
}
