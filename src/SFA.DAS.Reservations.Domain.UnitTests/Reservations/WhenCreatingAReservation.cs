using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenCreatingAReservation
    {

        [Test]
        public void ThenWillPopulatedDataFromIndexEntry()
        {
            //Arrange
            var fixture = new Fixture();
            var index = fixture.Build<ReservationIndex>()
                .Without(r => r.Course)
                .Without(r => r.CourseId)
                .Without(r => r.Id)
                .Create();

            //Act
            var reservation = new Reservation(index);

            //Assert
            reservation.Should().BeEquivalentTo(index, options => 
                options.Excluding(x => x.Id)
                       .Excluding(x => x.ReservationId));

            reservation.Id.Should().Be(index.ReservationId);
        }
    }
}
