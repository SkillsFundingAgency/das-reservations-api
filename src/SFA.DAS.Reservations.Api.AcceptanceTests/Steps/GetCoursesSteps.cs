using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class GetCoursesSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider)
        : StepsBase(testData, testResults, serviceProvider)
    {
        private List<Course> _collectedCourses;

        // Given

        [Given(@"there is an active course available")]
        public void GivenThereIsAnActiveCourseAvailable()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var course = dbContext.Courses.Single(c => c.CourseId.Equals(TestData.Course.CourseId));

            course.EffectiveTo = DateTime.UtcNow.AddDays(1);

            dbContext.Courses.Add(new Domain.Entities.Course
            {
                CourseId = "12345678", Level = 2, Title = "Expired Course", EffectiveTo = DateTime.UtcNow.AddDays(-1)
            });

            dbContext.SaveChanges();
        }

        [Given(@"there are no active courses available")]
        public void GivenThereIAreNoActiveCoursesAvailable()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var courses = dbContext.Courses.ToArray();

            foreach (var course in courses)
            {
                course.EffectiveTo = DateTime.UtcNow.AddDays(-1);
            }
            
            dbContext.SaveChanges();
        }

        // When

        [When(@"I get courses")]
        public void WhenIGetCourses()
        {
            var controller = Services.GetService<CoursesController>();

            var result = controller.GetAll().Result as OkObjectResult;

            var viewModel = result?.Value as CoursesViewModel;

            _collectedCourses = new List<Course>(viewModel?.Courses);
        }
        
        // Then

        [Then(@"the active course is returned")]
        public void ThenTheActiveCourseIsReturned()
        {
            _collectedCourses.Should().NotBeNullOrEmpty();
            _collectedCourses.Should().HaveCount(1);
            _collectedCourses[0].CourseId.Should().Be(TestData.Course.CourseId);
            _collectedCourses[0].Title.Should().Be(TestData.Course.Title);
            _collectedCourses[0].Level.Should().Be(TestData.Course.Level.ToString());
        }

        [Then(@"no courses are returned")]
        public void ThenNoCoursesAreReturned()
        {
            _collectedCourses.Should().NotBeNull();
            _collectedCourses.Should().BeEmpty();
        }
    }
}
