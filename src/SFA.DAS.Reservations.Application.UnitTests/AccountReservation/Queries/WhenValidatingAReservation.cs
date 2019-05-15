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
using SFA.DAS.Reservations.Domain.Validation;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;


namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries
{
    public class WhenValidatingAReservation
    {
        private const string CourseId = "123";
        private static readonly Guid ReservationId = Guid.NewGuid();

        private ValidateReservationQueryHandler _handler;
        private Mock<IAccountReservationService> _reservationService;
        private Mock<IValidator<ValidateReservationQuery>> _validator;
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

            _validator = new Mock<IValidator<ValidateReservationQuery>>();

            _validator.Setup(v => v.ValidateAsync(It.IsAny<ValidateReservationQuery>()))
                .ReturnsAsync(new ValidationResult());

            _handler = new ValidateReservationQueryHandler(_reservationService.Object, _validator.Object);
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
        public async Task Then_The_Query_Is_Validated()
        {
            //Assert
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(-1)
            };

            //Act
            await _handler.Handle(request, CancellationToken.None);

            //Assert
            _validator.Verify(x => x.ValidateAsync(request), Times.Once);

        }

        [Test]
        public void Then_If_The_Query_Fails_Validation_Then_An_Argument_Exception_Is_Thrown()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(-1)
            };

            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<ValidateReservationQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(request, CancellationToken.None));
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
            _courseRules.Add(new Rule
            {
                ActiveFrom = DateTime.Now.AddDays(-2), 
                ActiveTo = DateTime.Now.AddDays(2)
            });

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            var error = result?.Errors.FirstOrDefault();

            Assert.IsNotNull(error);
            Assert.AreEqual(nameof(ValidateReservationQuery.CourseId), error.PropertyName);
        }

        [Test]
        public async Task Then_Will_Return_No_Error_Messages_If_Course_Has_No_Active_Rules()
        {
            //Arrange
            var request = new ValidateReservationQuery
            {
                CourseId = CourseId,
                ReservationId = ReservationId,
                TrainingStartDate = _reservation.StartDate.AddDays(1)
            };
            _courseRules.Add(new Rule
            {
                ActiveFrom = DateTime.Now.AddDays(2), 
                ActiveTo = DateTime.Now.AddDays(4)
            });

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Errors);
        }
    }
}
