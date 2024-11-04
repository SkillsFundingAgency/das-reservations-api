using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.BulkCreateReservations
{
    public class WhenValidatingBulkCreateAccountReservations
    {
        [Test]
        public async Task Then_A_Valid_Command_Should_Pass()
        {
            //Arrange
            var command = new BulkCreateAccountReservationsCommand {ReservationCount = 2};
            var validator = new BulkCreateAccountReservationsCommandValidator();

            //Act
            var result = await validator.ValidateAsync(command);

            //Assert
            result.IsValid().Should().BeTrue();
        }

        [Test]
        public async Task Then_If_Values_Are_Not_Set_Should_Fail()
        {
            //Arrange
            var command = new BulkCreateAccountReservationsCommand();
            var validator = new BulkCreateAccountReservationsCommandValidator();

            //Act
            var result = await validator.ValidateAsync(command);

            //Assert
            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Should().HaveCount(1);
            result.ValidationDictionary.Should().ContainKey(nameof(command.ReservationCount));
        }
    }
}
