using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.ChangeOfParty
{
    public class WhenHandlingAChangeOfPartyCommand
    {
        [Test, MoqAutoData]
        public void And_Command_Fails_Validation_Then_Throws_ArgumentException(
            ChangeOfPartyCommand command,
            string propertyName,
            [Frozen] ValidationResult validationResult,
            ChangeOfPartyCommandHandler handler)
        {
            validationResult.AddError(propertyName);

            var act = new Func<Task>(async () => await handler.Handle(command, CancellationToken.None));

            act.Should().Throw<ArgumentException>()
                .WithMessage($"The following parameters have failed validation*{propertyName}*");
        }

        [Test, MoqAutoData]
        public async Task Then_Clones_Existing_Reservation_And_Returns_New_ReservationId(
            ChangeOfPartyCommand command,
            Guid newReservationId,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountReservationService> mockService,
            ChangeOfPartyCommandHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.ChangeOfParty(
                    It.Is<ChangeOfPartyServiceRequest>(request => 
                        request.ReservationId == command.ReservationId)))
                .ReturnsAsync(newReservationId);

            var result = await handler.Handle(command, CancellationToken.None);

            result.ReservationId.Should().Be(newReservationId);
        }
    }
}