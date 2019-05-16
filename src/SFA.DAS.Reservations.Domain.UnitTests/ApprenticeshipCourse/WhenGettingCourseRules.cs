using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Entities;
using Course = SFA.DAS.Reservations.Domain.ApprenticeshipCourse.Course;

namespace SFA.DAS.Reservations.Domain.UnitTests.ApprenticeshipCourse
{
    public class WhenGettingCourseRules
    {
        [Test]
        public void ThenWillReturnActiveRules()
        {
            //Arrange
            var trainingStartDate = DateTime.Now.AddDays(30);
            var course = new Course("1", "Test", "1");
            var activeRule = new Rule
            {
                ActiveFrom = trainingStartDate.AddDays(-2),
                ActiveTo = trainingStartDate.AddDays(2)
            };
            var unactiveRule = new Rule
            {
                ActiveFrom = trainingStartDate.AddDays(2),
                ActiveTo = trainingStartDate.AddDays(4)
            };

            course.Rules.Add(activeRule);
            course.Rules.Add(unactiveRule);

            //Act
            var result = course.GetActiveRules(trainingStartDate);

            //Assert
            Assert.IsNotNull(result);
            
            var rules = result as Rule[] ?? result.ToArray();

            Assert.AreEqual(1, rules.Length);
            Assert.AreEqual(activeRule, rules.First());
        }

        [Test]
        public void ThenWillEmptyCollectionIfNotActiveRules()
        {
            //Arrange
            var trainingStartDate = DateTime.Now.AddDays(30);
            var course = new Course("1", "Test", "1");
           
            var unactiveRule = new Rule
            {
                ActiveFrom = trainingStartDate.AddDays(2),
                ActiveTo = trainingStartDate.AddDays(4)
            };
           
            course.Rules.Add(unactiveRule);

            //Act
            var result = course.GetActiveRules(trainingStartDate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
