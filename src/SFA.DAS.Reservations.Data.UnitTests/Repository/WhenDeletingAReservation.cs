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
        [Test, RecursiveMoqAutoData]
        public async Task Then_Updates_Status_To_Deleted(
            List<Reservation> reservations,
            [Frozen] Mock<IReservationsDataContext> mockContext,
            ReservationRepository repository)
        {
            var reservationId = reservations[0].Id;
            reservations[0].Status = (int) ReservationStatus.Pending;
            mockContext
                .Setup(context => context.Reservations)
                .ReturnsDbSet(reservations);
            mockContext
                .Setup(context => context.Reservations.FindAsync(reservationId))
                .ReturnsAsync(reservations[0]);

            await repository.DeleteAccountReservation(reservationId);

            mockContext.Verify(context => context.SaveChanges(), Times.Once);
            reservations[0].Status.Should().Be((int) ReservationStatus.Deleted);
        }

        [Test, RecursiveMoqAutoData]
        public void And_No_Reservation_To_Delete_Then_Throws_EntityNotFoundException(
            List<Reservation> reservations,
            [Frozen] Mock<IReservationsDataContext> mockContext,
            ReservationRepository repository)
        {
            var reservationId = reservations[0].Id;
            mockContext
                .Setup(context => context.Reservations.FindAsync(reservationId))
                .ReturnsAsync((Reservation)null);

            var act = new Func<Task>(async () => await repository.DeleteAccountReservation(reservationId));

            act.Should().Throw<EntityNotFoundException<Reservation>>();
        }

        [Test, RecursiveMoqAutoData]
        public void And_ReservationStatus_Not_Pending_Then_Throws_InvalidOperation(
            List<Reservation> reservations,
            [Frozen] Mock<IReservationsDataContext> mockContext,
            ReservationRepository repository)
        {
            var reservationId = reservations[0].Id;
            reservations[0].Status = (int) ReservationStatus.Confirmed;
            mockContext
                .Setup(context => context.Reservations.FindAsync(reservationId))
                .ReturnsAsync(reservations[0]);

            var act = new Func<Task>(async () => await repository.DeleteAccountReservation(reservationId));

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"This reservation cannot be deleted");
        }
    }
}