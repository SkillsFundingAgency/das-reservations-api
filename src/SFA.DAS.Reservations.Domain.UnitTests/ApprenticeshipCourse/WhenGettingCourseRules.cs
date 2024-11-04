using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using Course = SFA.DAS.Reservations.Domain.ApprenticeshipCourse.Course;

namespace SFA.DAS.Reservations.Domain.UnitTests.ApprenticeshipCourse
{
    public class WhenGettingCourseRules
    {
        [TestCase("2019-01-30","2019-02-20", 1)] 
        [TestCase("2019-02-10","2019-03-20", 1)] 
        [TestCase("2019-02-05","2019-02-20", 1)] 
        [TestCase("2019-01-10","2019-04-20", 1)] 
        [TestCase("2018-11-30","2019-01-01", 0)]
        [TestCase("2019-04-10","2019-05-20", 0)]
        public void ThenWillReturnActiveRules(string ruleActiveFromDate, string ruleActiveToDate, int expectedRuleCount)
        {
            //Arrange
            var reservationDates = new ReservationDates
            {
                TrainingStartDate = new DateTime(2019, 2, 1),
                ReservationStartDate = new DateTime(2019, 1, 20),
                ReservationExpiryDate = new DateTime(2019, 3, 1),
                ReservationCreatedDate = new DateTime(2019, 1, 18)
            };

            var course = new Course("1", "Test", "1", DateTime.Today);

            var activeRule = new Rule
            {
                ActiveFrom = DateTime.Parse(ruleActiveFromDate),
                ActiveTo = DateTime.Parse(ruleActiveToDate)
            };
            course.Rules.Add(activeRule);

            //Act
            var result = course.GetActiveRules(reservationDates);

            //Assert
            result.Should().NotBeNull();

            var rules = result as Rule[] ?? result.ToArray();
            rules.Length.Should().Be(expectedRuleCount);
        }

        [Test]
        public void ThenWillEmptyCollectionIfNotActiveRules()
        {
            //Arrange
            var reservationDates = new ReservationDates
            {
                TrainingStartDate = DateTime.Now.AddDays(30),
                ReservationStartDate = DateTime.Now.AddDays(20),
                ReservationExpiryDate = DateTime.Now.AddDays(40),
                ReservationCreatedDate = DateTime.Now.AddDays(18)
            };
            
            var course = new Course("1", "Test", "1", DateTime.Today);

            var inactiveRule = new Rule
            {
                ActiveFrom = DateTime.Now.AddDays(-2),
                ActiveTo = DateTime.Now.AddDays(-4)
            };

            course.Rules.Add(inactiveRule);

            //Act
            var result = course.GetActiveRules(reservationDates);

            //Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void ThenWillNotIncludeRulesCreatedAfterReservationStartDate()
        {
            //Arrange
            var reservationDates = new ReservationDates
            {
                TrainingStartDate = DateTime.Now.AddDays(10),
                ReservationStartDate = DateTime.Now,
                ReservationExpiryDate = DateTime.Now.AddDays(20),
                ReservationCreatedDate = DateTime.Now.AddDays(-5)
            };
           
            var course = new Course("1", "Test", "1", DateTime.Today);

            var inactiveRule = new Rule
            {
                CreatedDate = DateTime.Now,
                ActiveFrom = DateTime.Now.AddDays(5),
                ActiveTo = DateTime.Now.AddDays(15)
            };

            course.Rules.Add(inactiveRule);

            //Act
            var result = course.GetActiveRules(reservationDates);

            //Assert
            result.Should().BeEmpty();
        }
    }
}
