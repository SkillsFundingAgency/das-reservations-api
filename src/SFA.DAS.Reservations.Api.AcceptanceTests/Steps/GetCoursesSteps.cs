using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class GetCoursesSteps : StepsBase
    {
        private List<Course> _collectedCourses;

        public GetCoursesSteps(TestData testData, TestServiceProvider serviceProvider) : base(testData, serviceProvider)
        {
            
        }

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
            Assert.IsNotNull(_collectedCourses);
            Assert.IsNotEmpty(_collectedCourses);
            Assert.AreEqual(1, _collectedCourses.Count);
            Assert.AreEqual(TestData.Course.CourseId, _collectedCourses.First().CourseId);
            Assert.AreEqual(TestData.Course.Title, _collectedCourses.First().Title);
            Assert.AreEqual(TestData.Course.Level.ToString(), _collectedCourses.First().Level);
        }

        [Then(@"no courses are returned")]
        public void ThenNoCoursesAreReturned()
        {
            Assert.IsNotNull(_collectedCourses);
            Assert.IsEmpty(_collectedCourses);
        }
    }
}
