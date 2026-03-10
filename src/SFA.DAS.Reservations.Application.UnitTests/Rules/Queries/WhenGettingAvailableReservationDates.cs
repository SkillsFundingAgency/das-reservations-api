using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Application.Rules.Queries.GetAvailableDates;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Types;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries;

[TestFixture]
public class WhenGettingAvailableReservationDates
{
    [Test, MoqAutoData]
    public async Task And_Query_Invalid_Then_Throws_InvalidArgumentException(
        GetAvailableDatesQuery query,
        ValidationResult validationResult,
        string propertyName,
        List<AvailableDateStartWindow> availableDateStartWindows,
        [Frozen] Mock<IValidator<GetAvailableDatesQuery>> mockValidator,
        [Frozen] Mock<IAccountLegalEntitiesService> mockAleService,
        [Frozen] Mock<IAvailableDatesService> mockDatesService,
        [Frozen] Mock<ICourseService> mockCourseService,
        [Frozen] Mock<IAvailableDatesRestrictionProvider> mockRestrictionProvider,
        GetAvailableDatesQueryHandler handler)
    {
        validationResult.AddError(propertyName);
        mockValidator
            .Setup(validator => validator.ValidateAsync(query))
            .ReturnsAsync(validationResult);

        var act = new Func<Task>(async ()  => await handler.Handle(query, CancellationToken.None));

        var ex = await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("The following parameters have failed validation*");
                
        ex.Subject.Single().ParamName.Should().Contain(propertyName);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Available_Dates_Service_Is_Called_And_Dates_Returned(
        GetAvailableDatesQuery query,
        [Frozen] ValidationResult validationResult,
        AccountLegalEntity accountLegalEntity,
        AvailableDateStartWindow previousMonth,
        List<AvailableDateStartWindow> availableDateStartWindows,
        [Frozen] Mock<IAccountLegalEntitiesService> mockAleService,
        [Frozen] Mock<IAvailableDatesService> mockDatesService,
        [Frozen] Mock<ICourseService> mockCourseService,
        [Frozen] Mock<IAvailableDatesRestrictionProvider> mockRestrictionProvider,
        GetAvailableDatesQueryHandler handler)
    {
        //Arrange
        query.CourseId = null;
        validationResult.ValidationDictionary.Clear();
        mockAleService
            .Setup(service => service.GetAccountLegalEntity(query.AccountLegalEntityId))
            .ReturnsAsync(accountLegalEntity);
        mockDatesService
            .Setup(x => x.GetAvailableDates(It.IsAny<bool>()))
            .Returns(availableDateStartWindows);

        //Act
        var actual = await handler.Handle(query, CancellationToken.None);

        //Assert
        actual.AvailableDates.Should().BeEquivalentTo(availableDateStartWindows);
    }

    [Test, MoqAutoData]
    public async Task When_CourseId_Provided_And_Course_Is_ApprenticeshipUnit_Then_Service_Called_With_Restrictions_Excluding_Previous_Month(
        GetAvailableDatesQuery query,
        [Frozen] ValidationResult validationResult,
        AccountLegalEntity accountLegalEntity,
        List<AvailableDateStartWindow> availableDateStartWindows,
        [Frozen] Mock<IAccountLegalEntitiesService> mockAleService,
        [Frozen] Mock<IAvailableDatesService> mockDatesService,
        [Frozen] Mock<ICourseService> mockCourseService,
        [Frozen] Mock<IAvailableDatesRestrictionProvider> mockRestrictionProvider,
        GetAvailableDatesQueryHandler handler)
    {
        // Arrange
        var courseId = "123";
        query.CourseId = courseId;
        validationResult.ValidationDictionary.Clear();
        var course = new Course("123", "Test", "4", null, "ApprenticeshipUnit", LearningType.ApprenticeshipUnit);

        mockAleService.Setup(s => s.GetAccountLegalEntity(query.AccountLegalEntityId)).ReturnsAsync(accountLegalEntity);
        mockCourseService.Setup(s => s.GetCourseById(courseId)).ReturnsAsync(course);
        mockRestrictionProvider.Setup(s => s.IncludePreviousMonth(LearningType.ApprenticeshipUnit)).Returns(false);
        mockDatesService.Setup(x => x.GetAvailableDates(It.IsAny<bool>())).Returns(availableDateStartWindows);

        // Act
        var actual = await handler.Handle(query, CancellationToken.None);

        // Assert
        actual.AvailableDates.Should().BeEquivalentTo(availableDateStartWindows);
        mockRestrictionProvider.Verify(s => s.IncludePreviousMonth(LearningType.ApprenticeshipUnit), Times.Once);
        mockDatesService.Verify(x => x.GetAvailableDates(false), Times.Once);
    }
}