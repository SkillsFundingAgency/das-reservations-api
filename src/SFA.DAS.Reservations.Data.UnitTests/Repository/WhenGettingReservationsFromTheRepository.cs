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
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingReservationsFromTheRepository
    {
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private ReservationRepository _reservationRepository;
        private Reservation[] _reservations;

        [SetUp]
        public void Arrange()
        {
            var today = DateTime.UtcNow;


            _reservations = new []
            {
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "A Team",
                    Course = new Course {Title = "Book Keeping", Level = 1},
                    CreatedDate = today
                },
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "A Team",
                    Course = new Course {Title = "Plant Engineering", Level = 1},
                    CreatedDate = today
                },
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "B Team",
                    Course = new Course {Title = "Accounting", Level = 1},
                    CreatedDate = today

                },
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "B Team",
                    Course = new Course {Title = "Accounting", Level = 2},
                    CreatedDate = today
                },
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "C Team",
                    Course = new Course {Title = "Computing", Level = 1},
                    CreatedDate = today,
                    StartDate = today.AddDays(3)
                },
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "C Team",
                    Course = new Course {Title = "Computing", Level = 1},
                    CreatedDate = today,
                    StartDate = today.AddDays(1)

                },
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "A Team",
                    Course = new Course {Title = "Accounting", Level = 3},
                    Status = (int)ReservationStatus.Deleted
                },
                new Reservation
                {
                    AccountId = 1,
                    AccountLegalEntityName = "A Team",
                    Course = new Course {Title = "Accounting", Level = 3},
                    Status = (int)ReservationStatus.Change
                },
                new Reservation
                {
                    AccountId = 2,
                    AccountLegalEntityName = "A Team",
                    Course = new Course {Title = "Accounting", Level = 1},
                    CreatedDate = today
                },
            };

            var unorderedReservations = new List<Reservation>
            { 
                _reservations[7],
                _reservations[6],
                _reservations[5],
                _reservations[4],
                _reservations[3],
                _reservations[2],
                _reservations[1],
                _reservations[0],
            };
         
            _reservationsDataContext = new Mock<IReservationsDataContext>();
           
            _reservationsDataContext.Setup(x => x.Reservations).ReturnsDbSet(unorderedReservations);

            _reservationRepository = new ReservationRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task Then_The_Results_Are_Filtered_By_Account_Id()
        {
            //Act
            var actual = await _reservationRepository.GetAccountReservations(1);

            //Assert
            actual.Should().HaveCount(6);
        }

        [Test]
        public async Task Then_Deleted_Reservations_Are_Not_Returned()
        {
            var actual = await _reservationRepository.GetAccountReservations(1);

            var deletedCount = actual.Count(reservation => reservation.Status == (int) ReservationStatus.Deleted);
            deletedCount.Should().Be(0);
        }

        [Test]
        public async Task Then_ChangeOfParty_Reservations_Are_Not_Returned()
        {
            var actual = await _reservationRepository.GetAccountReservations(1);

            var count = actual.Count(reservation => reservation.Status == (int) ReservationStatus.Change);
            count.Should().Be(0);
        }

        [Test]
        public async Task Then_An_Empty_List_Is_Returned_When_There_Are_No_Records()
        {
            //Act
            var actual = await _reservationRepository.GetAccountReservations(3);

            //Assert
            actual.Should().BeEmpty();
        }
    }
}
