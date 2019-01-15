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
            var actualApprenticeship = new Apprenticeship
            {
                CourseId = courseId
            };

            //Assert
            var expectedType = isStandard ? ApprenticeshipType.Standard : ApprenticeshipType.Framework;
            Assert.AreEqual(expectedType, actualApprenticeship.Type);
        }

        [Test]
        public void Then_The_Course_Description_Is_Taken_From_The_Title_And_Level()
        {
            //Arrange Act
            var actualApprenticeship = new Apprenticeship
            {
                Title = "Some title",
                Level = "1"
            };

            //Assert
            Assert.AreEqual("Some title - Level 1", actualApprenticeship.CourseDescription);
        }
    }
}
