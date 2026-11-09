using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;
using Course = SFA.DAS.Reservations.Domain.ApprenticeshipCourse.Course;
using SFA.DAS.Reservations.Domain.Types;


namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries;

public class WhenValidatingAReservationWithLearningType
{
    private const string CourseId = "123";
    private static readonly Guid ReservationId = Guid.NewGuid();

    private ValidateReservationQueryHandler _handler;
    private Mock<IAccountReservationService> _reservationService;
    private Mock<ICourseService> _courseService;
    private Mock<IValidator<ValidateReservationQuery>> _validator;
    private Reservation _reservation;
    private Course _course;
    private Course _oldCourse;

    public void SetupLearningTypes(LearningType? oldLearningType, LearningType? newLearningType)
    {
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        _reservation = new Reservation(time => Task.FromResult(new List<Rule>() as IList<Rule>), ReservationId, 1, false, DateTime.Now, startDate, startDate.AddMonths(3), ReservationStatus.Pending,
            new Domain.Entities.Course { CourseId = "2221", Title = "Some Other Course", LearningType = oldLearningType }, 1, 1, "Legal Entity", 0, null);

        _reservationService = new Mock<IAccountReservationService>();
        _reservationService
            .Setup(r => r.GetReservation(It.IsAny<Guid>()))
            .ReturnsAsync(_reservation);

        _oldCourse = new Course(CourseId, "Some Other Course", "1", DateTime.Today, "Apprenticeship", oldLearningType);
        _course = new Course(CourseId, "Another Test Course", "1", DateTime.Today, "Apprenticeship", newLearningType);

        _courseService = new Mock<ICourseService>();
        _courseService.Setup(s => s.GetCourseById(_reservation.Course.CourseId))
            .ReturnsAsync(_oldCourse);
        _courseService.Setup(s => s.GetCourseById(CourseId))
            .ReturnsAsync(_course);

        _validator = new Mock<IValidator<ValidateReservationQuery>>();
        _validator.Setup(v => v.ValidateAsync(It.IsAny<ValidateReservationQuery>()))
            .ReturnsAsync(new ValidationResult());

        _handler = new ValidateReservationQueryHandler(_reservationService.Object, _courseService.Object, _validator.Object);
    }

    [Test]
    public async Task Then_Not_Check_LearningType_When_CourseCode_Is_The_Same()
    {
        //Arrange
        SetupLearningTypes(null, null);
        var request = new ValidateReservationQuery
        {
            CourseCode = _reservation.Course.CourseId,
            ReservationId = ReservationId,
            StartDate = _reservation.StartDate.Value.AddDays(1)
        };

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Errors.Should().BeEmpty();
    }

    [TestCase(LearningType.ApprenticeshipUnit, LearningType.Apprenticeship, "Choose a learner doing an apprenticeship unit to use this reservation")]
    [TestCase(LearningType.Apprenticeship, LearningType.ApprenticeshipUnit, "Choose a learner doing an apprenticeship to use this reservation")]
    public async Task Then_Will_Raise_Appropriate_Error_When_Change_Of_LearningType_Is_Invalid(LearningType oldLearningType, LearningType newLearningType, string expectedReason)
    {
        //Arrange
        SetupLearningTypes(oldLearningType, newLearningType);

        var request = new ValidateReservationQuery
        {
            CourseCode = CourseId,
            ReservationId = ReservationId,
            StartDate = _reservation.StartDate.Value.AddDays(1)
        };

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().PropertyName.Should().Be(nameof(request.CourseCode));
        result.Errors.First().Reason.Should().Be(expectedReason);
    }

    [TestCase(LearningType.ApprenticeshipUnit, LearningType.ApprenticeshipUnit)]
    [TestCase(LearningType.Apprenticeship, LearningType.Apprenticeship)]
    [TestCase(LearningType.FoundationApprenticeship, LearningType.FoundationApprenticeship)]
    [TestCase(LearningType.Apprenticeship, LearningType.FoundationApprenticeship)]
    [TestCase(LearningType.FoundationApprenticeship, LearningType.Apprenticeship)]
    public async Task Then_Will_Be_Valid_When_Change_Of_LearningType_Is_Acceptable(LearningType oldLearningType, LearningType newLearningType)
    {
        //Arrange
        SetupLearningTypes(oldLearningType, newLearningType);

        var request = new ValidateReservationQuery
        {
            CourseCode = CourseId,
            ReservationId = ReservationId,
            StartDate = _reservation.StartDate.Value.AddDays(1)
        };

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Errors.Should().BeEmpty();
    }
}