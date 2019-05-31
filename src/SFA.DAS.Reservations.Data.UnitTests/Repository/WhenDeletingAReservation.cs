using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    [TestFixture]
    public class WhenDeletingAReservation
    {
        List<Reservation> _reservations;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _reservations = fixture.Create<List<Reservation>>();
        }

        [Test, MoqAutoData]
        public async Task Then_Updates_Status_To_Deleted(
            [Frozen] Mock<IReservationsDataContext> mockContext,
            ReservationRepository repository)
        {
            var reservationId = _reservations[0].Id;
            mockContext
                .Setup(context => context.Reservations)
                .ReturnsDbSet(_reservations);
            mockContext
                .Setup(context => context.Reservations.FindAsync(reservationId))
                .ReturnsAsync(_reservations[0]);

            await repository.DeleteAccountReservation(reservationId);

            mockContext.Verify(context => context.SaveChanges(), Times.Once);
            _reservations[0].Status.Should().Be((int) ReservationStatus.Deleted);
        }

        [Test, MoqAutoData]
        public void And_No_Reservation_To_Delete_Then_Throws_EntityNotFoundException(
            [Frozen] Mock<IReservationsDataContext> mockContext,
            ReservationRepository repository)
        {
            var reservationId = _reservations[0].Id;
            mockContext
                .Setup(context => context.Reservations.FindAsync(reservationId))
                .ReturnsAsync((Reservation)null);

            var act = new Func<Task>(async () => await repository.DeleteAccountReservation(reservationId));

            act.Should().Throw<EntityNotFoundException>()
                .WithMessage($"Entity not found [{nameof(Reservation)}], id: [{reservationId}]");
        }
    }
}