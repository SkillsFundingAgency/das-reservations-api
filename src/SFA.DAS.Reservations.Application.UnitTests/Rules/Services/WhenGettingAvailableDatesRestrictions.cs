using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Types;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services;

public class WhenGettingAvailableDatesRestrictions
{
    private AvailableDatesRestrictionProvider _sut;

    [SetUp]
    public void Arrange()
    {
        _sut = new AvailableDatesRestrictionProvider();
    }

    [Test]
    public void Then_ApprenticeshipUnit_Returns_False()
    {
        _sut.IncludePreviousMonth(LearningType.ApprenticeshipUnit).Should().BeFalse();
    }

    [TestCase(null)]
    [TestCase(LearningType.Apprenticeship)]
    [TestCase(LearningType.FoundationApprenticeship)]
    public void Then_Other_LearningTypes_Return_True(LearningType? learningType)
    {
        _sut.IncludePreviousMonth(learningType).Should().BeTrue();
    }
}