using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Data;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Types;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class AvailableDatesSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider)
        : StepsBase(testData, testResults, serviceProvider)
    {
        private const int DefaultAvailableDatesMonthCount = 6;
        private GetAvailableDatesResult _availableDatesResult;

        [Given(@"I have signed an Agreement")]
        public void GivenIHaveSignedAnAgreement()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var legalEntity = dbContext.AccountLegalEntities.Single(le => le.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            legalEntity.AgreementSigned = true;

            dbContext.SaveChanges();
        }
        
        [Given(@"the course is an Apprenticeship Unit")]
        public void GivenTheCourseIsAnApprenticeshipUnit()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var course = dbContext.Courses.Single(c => c.CourseId == TestData.Course.CourseId);
            course.LearningType = LearningType.ApprenticeshipUnit;
            dbContext.SaveChanges();
        }

        [Given(@"the course is not an Apprenticeship Unit")]
        public void GivenTheCourseIsNotAnApprenticeshipUnit()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var course = dbContext.Courses.Single(c => c.CourseId == TestData.Course.CourseId);
            course.LearningType = LearningType.Apprenticeship;
            dbContext.SaveChanges();
        }

        [When(@"I get available dates")]
        public void WhenIGetAvailableDates()
        {
            WhenIGetAvailableDatesForCourse(null);
        }

        [When(@"I get available dates for course ""(.*)""")]
        public void WhenIGetAvailableDatesForCourse(string courseId)
        {
            var controller = Services.GetService<RulesController>();
            var result = controller.GetAvailableDates(TestData.AccountLegalEntity.AccountLegalEntityId, courseId).Result as OkObjectResult;
            _availableDatesResult = result?.Value as GetAvailableDatesResult;
        }

        [Then(@"I should get available dates back")]
        public void ThenIShouldGetStandardAvailableDatesBack()
        {
            var currentDateTime = Services.GetService<ICurrentDateTime>();
            _availableDatesResult.AvailableDates.Count().Should().Be(7);
            
            var expectedDates = GetExpectedDates(DefaultAvailableDatesMonthCount, currentDateTime.GetDate(),  DateTime.MaxValue);
            
            _availableDatesResult.AvailableDates.Should().BeEquivalentTo(expectedDates);
        }

        [Then(@"the available dates should not include the previous month")]
        public void ThenTheAvailableDatesShouldNotIncludeThePreviousMonth()
        {
            var currentDateTime = Services.GetService<ICurrentDateTime>();
            var now = currentDateTime.GetDate();
            var firstDayOfCurrentMonth = new DateTime(now.Year, now.Month, 1);

            _availableDatesResult.AvailableDates.Should().NotBeEmpty();
            _availableDatesResult.AvailableDates.First().StartDate.Should().Be(firstDayOfCurrentMonth);
            _availableDatesResult.AvailableDates.Count().Should().Be(DefaultAvailableDatesMonthCount);
        }

        [Then(@"the first available date should be the previous month")]
        public void ThenTheFirstAvailableDateShouldBeThePreviousMonth()
        {
            var currentDateTime = Services.GetService<ICurrentDateTime>();
            var now = currentDateTime.GetDate();
            var previousMonth = now.AddMonths(-1);
            var firstDayOfPreviousMonth = new DateTime(previousMonth.Year, previousMonth.Month, 1);

            _availableDatesResult.AvailableDates.Should().NotBeEmpty();
            _availableDatesResult.AvailableDates.First().StartDate.Should().Be(firstDayOfPreviousMonth);
        }

        private static IEnumerable<AvailableDateStartWindow> GetExpectedDates(int expectedMonthCount, DateTime minDate, DateTime maxDate)
        {
            var expectedDates = new List<AvailableDateStartWindow>();

            var previousMonth = minDate.AddMonths(-1);
            expectedDates.Add(new AvailableDateStartWindow
            {
                StartDate = new DateTime(previousMonth.Year, previousMonth.Month, 1),
                EndDate = new DateTime(previousMonth.AddMonths(2).Year, previousMonth.AddMonths(2).Month,
                    DateTime.DaysInMonth(previousMonth.AddMonths(2).Year, previousMonth.AddMonths(2).Month))
            });

            for (var index = 0; index < expectedMonthCount; index++)
            {
                var startDate = minDate.AddMonths(index);

                if (startDate > maxDate)
                    break;

                var endDate = startDate.AddMonths(2);

                var newDateWindow = new AvailableDateStartWindow
                {
                    StartDate = new DateTime(startDate.Year, startDate.Month, 1),
                    EndDate = new DateTime(endDate.Year, endDate.Month,
                        DateTime.DaysInMonth(endDate.Year, endDate.Month))
                };

                expectedDates.Add(newDateWindow);
            }

            return expectedDates;
        }
    }
}
