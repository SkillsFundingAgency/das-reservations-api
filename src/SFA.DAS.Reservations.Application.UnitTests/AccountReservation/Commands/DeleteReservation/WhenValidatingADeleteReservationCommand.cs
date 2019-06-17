using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.DeleteReservation
{
    [TestFixture]
    public class WhenValidatingADeleteReservationCommand
    {
        [Test, AutoData]
        public async Task And_Default_Guid_Then_Not_Valid(
            DeleteReservationCommandValidator validator)
        {
            var command = new DeleteReservationCommand();

            var result = await validator.ValidateAsync(command);

            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Keys.Should().Contain(nameof(command.ReservationId));
        }

        [Test, AutoData]
        public async Task And_Guid_Then_Is_Valid(
            DeleteReservationCommand command,
            DeleteReservationCommandValidator validator)
        {
            var result = await validator.ValidateAsync(command);

            result.IsValid().Should().BeTrue();
        }
    }
}