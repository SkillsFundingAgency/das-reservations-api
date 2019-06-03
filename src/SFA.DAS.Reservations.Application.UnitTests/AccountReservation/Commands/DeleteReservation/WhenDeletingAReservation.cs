using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Reservations.Domain.Validation.ValidationResult;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.DeleteReservation
{
    [TestFixture]
    public class WhenDeletingAReservation
    {
        [Test, MoqAutoData]
        public void And_Command_Fails_Validation_Then_Throws_ValidationException(
            DeleteReservationCommand command,
            string propertyName,
            [Frozen] ValidationResult validationResult,
            DeleteReservationCommandHandler handler)
        {
            validationResult.AddError(propertyName);

            var act = new Func<Task>(async () => await handler.Handle(command, CancellationToken.None));

            act.Should().Throw<ArgumentException>()
                .WithMessage($"The following parameters have failed validation*{propertyName}*");
        }

        [Test, MoqAutoData]
        public async Task Then_Calls_Service_To_Delete_Reservation(
            DeleteReservationCommand command,
            string propertyName,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountReservationService> mockService,
            DeleteReservationCommandHandler handler)
        {
            validationResult.ValidationDictionary.Clear();

            await handler.Handle(command, CancellationToken.None);

            mockService
                .Verify(service => service.DeleteReservation(command.ReservationId),
                Times.Once);
        }
    }
}