using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer.Query.ExpressionTranslators.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingReservationsFromTheRepository
    {
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private ReservationRepository _reservationRepository;

        [SetUp]
        public void Arrange()
        {
            var rules = new List<Reservation>
            {
                new Reservation
                {
                    AccountId = 1,
                    CreatedDate = DateTime.UtcNow
                },
                new Reservation
                {
                    AccountId = 1,
                    CreatedDate = DateTime.UtcNow
                },
                new Reservation
                {
                    AccountId = 2,
                    CreatedDate = DateTime.UtcNow
                },
                new Reservation
                {
                    AccountId = 1,
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)ReservationStatus.Deleted
                }
            };

            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Reservations).ReturnsDbSet(rules);

            _reservationRepository = new ReservationRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task Then_The_Results_Are_Filtered_By_Account_Id()
        {
            //Act
            var actual = await _reservationRepository.GetAccountReservations(1);

            //Assert
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task Then_Deleted_Reservations_Are_Not_Returned()
        {
            var actual = await _reservationRepository.GetAccountReservations(1);

            var deletedCount = actual.Count(reservation => reservation.Status == (int) ReservationStatus.Deleted);
            Assert.AreEqual(0, deletedCount);
        }

        [Test]
        public async Task Then_An_Empty_List_Is_Returned_When_There_Are_No_Records()
        {
            //Act
            var actual = await _reservationRepository.GetAccountReservations(3);

            //Assert
            Assert.IsEmpty(actual);
        }

    }
}
