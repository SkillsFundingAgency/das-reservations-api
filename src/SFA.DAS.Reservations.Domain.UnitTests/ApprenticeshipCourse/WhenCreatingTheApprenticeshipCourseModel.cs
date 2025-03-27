using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Domain.UnitTests.ApprenticeshipCourse
{
    public class WhenCreatingTheApprenticeshipCourseModel
    {
        [TestCase("1", true)]
        [TestCase("1-23-1", false)]
        [TestCase("1-23", false)]
        [TestCase("", true)]
        [TestCase("x", true)]
        public void Then_The_ApprenticeshipType_Is_Correctly_Worked_Out_From_The_Course_Id(string courseId, bool isStandard)
        {
            //Arrange Act
            var actualApprenticeship = new Course(courseId, "", "", DateTime.Today, "Apprenticeship");
            
            //Assert
            var expectedType = isStandard ? ApprenticeshipType.Standard : ApprenticeshipType.Framework;
            actualApprenticeship.Type.Should().Be(expectedType);
        }
        
    }
}
