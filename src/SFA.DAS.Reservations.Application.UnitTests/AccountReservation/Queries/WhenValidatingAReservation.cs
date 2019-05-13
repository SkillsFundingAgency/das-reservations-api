using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;


namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries
{
    public class WhenValidatingAReservation
    {
        private const string CourseId = "123";
        private static readonly Guid ReservationId = Guid.NewGuid();

        private ValidateReservationQueryHandler _handler;
        private Mock<IAccountReservationService> _reservationService;
        private Reservation _reservation;
        
        private IList<Rule> _courseRules;
        private Course _course;


        [SetUp]
        public void Arrange()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            _courseRules = new List<Rule>();

            _course = new Course
            {
                CourseId = CourseId,
                Title = "Test Course",
                Level = 1,
                ReservationRule = _courseRules
            };

            _reservation = new Reservation(time => Task.FromResult(new List<Rule>() as IList<Rule>), ReservationId, 1, true, DateTime.Now, startDate, startDate.AddMonths(3), ReservationStatus.Pending, _course, 1, 1, "Legal Entity");

            _reservationService = new Mock<IAccountReservationService>();

            _reservationService.Setup(r => r.GetReservation(It.IsAny<Guid>()))
                .ReturnsAsync(_reservation);

            _handler = new ValidateReservationQueryHandler(_reservationService.Object);
        }

        [Test]
        public async Task Then_Will_Return_No_Errors_Reservation_Is_Valid()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(1)
            };

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public async Task Then_Will_Return_Error_If_Training_Start_Date_Is_Before_Reservation_Start_Date()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(-1)
            };

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        public async Task Then_Will_Return_Error_Message_If_Training_Start_Date_Is_Before_Reservation_Start_Date()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(-1)
            };

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            var error = result?.Errors.FirstOrDefault();

            Assert.IsNotNull(error);
            Assert.AreEqual(nameof(ValidateReservationQuery.TrainingStartDate), error.PropertyName);
        }

        [Test]
        public async Task Then_Will_Return_Error_If_Training_Start_Date_Is_After_Reservation_Expiry_Date()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.ExpiryDate.AddDays(1)
            };

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        public async Task Then_Will_Return_Error_Message_If_Training_Start_Date_Is_After_Reservation_Expiry_Date()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.ExpiryDate.AddDays(1)
            };

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            var error = result?.Errors.FirstOrDefault();

            Assert.IsNotNull(error);
            Assert.AreEqual(nameof(ValidateReservationQuery.TrainingStartDate), error.PropertyName);
        }

        [Test]
        public async Task Then_Will_Return_Error_If_Course_Has_Active_Rules()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(1)
            };
            _courseRules.Add(new Rule());

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        public async Task Then_Will_Return_Error_Message_If_Course_Has_Active_Rules()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(1)
            };
            _courseRules.Add(new Rule());

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            var error = result?.Errors.FirstOrDefault();

            Assert.IsNotNull(error);
            Assert.AreEqual(nameof(ValidateReservationQuery.CourseId), error.PropertyName);
        }
    }
}
