using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingAReservation
    {
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private ReservationRepository _reservationRepository;

        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private const long ExpectedAccountId = 5548;

        [SetUp]
        public void Arrange()
        {
            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    Id = _expectedReservationId,
                    AccountId = ExpectedAccountId,
                    CreatedDate = DateTime.UtcNow
                },
                new Reservation
                {
                    Id = Guid.NewGuid(),
                    AccountId = 9,
                    CreatedDate = DateTime.UtcNow
                },
                new Reservation
                {
                    Id = Guid.NewGuid(),
                    AccountId = 2,
                    CreatedDate = DateTime.UtcNow
                }
            };

            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Reservations).ReturnsDbSet(reservations);
            _reservationsDataContext.Setup(x => x.Reservations.FindAsync(_expectedReservationId))
                .ReturnsAsync(reservations.FirstOrDefault(c => c.Id.Equals(_expectedReservationId)));
            
            _reservationRepository = new ReservationRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task Then_The_Reservation_Is_Returned_By_Id()
        {
            //Act
            var actual = await _reservationRepository.GetById(_expectedReservationId);

            //Assert
            actual.Should().NotBeNull();
            actual.AccountId.Should().Be(ExpectedAccountId);
        }

        [Test]
        public async Task Then_Null_Is_Returned_If_No_Record_Is_Found()
        {
            //Act
            var actual = await _reservationRepository.GetById(Guid.NewGuid());

            //Assert
            actual.Should().BeNull();
        }
    }
}
