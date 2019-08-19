using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;
using Course = SFA.DAS.Reservations.Domain.Entities.Course;
using Reservation = SFA.DAS.Reservations.Api.Models.Reservation;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    [Binding]
    public class CreateReservationSteps
    {
        private readonly TestData _testData;
        private readonly IServiceProvider _services;

        public CreateReservationSteps(TestData testData, TestServiceProvider serviceProvider)
        {
            _testData = testData;
            _services = serviceProvider;
        }

        [Given(@"I have a non levy account")]
        public void GivenIHaveANonLevyAccount()
        {
            _testData.IsLevyAccount = false;
        }

        [Given(@"I have a levy account")]
        public void GivenIHaveALevyAccount()
        {
            _testData.IsLevyAccount = true;
        }

        [Given(@"a course name (.*) has been added to the course list")]
        public void GivenACourseNameHasBeenAddedToTheCourseList(string courseName)
        {
            var dbContext = _services.GetService<ReservationsDataContext>();

            var course = new Course
            {
                CourseId = "234",
                Level = 1,
                Title = courseName
            };

            dbContext.Courses.Add(course);

            _testData.Courses.Add(courseName, course);

            dbContext.AccountLegalEntities.Add(new AccountLegalEntity
            {
                AccountLegalEntityId = 1,
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                AgreementSigned = true
            });

            dbContext.SaveChanges();
        }
        
        [When(@"I create a reservation for the (.*) course with a start month of (.*)")]
        public void WhenICreateAReservationForTheCourseWithAStartMonth(string courseName, string startMonth)
        {
            var month = (int) Enum.Parse<Month>(startMonth);
           
            var expectedCourse = _testData.Courses[courseName];

            var controller = _services.GetService<ReservationsController>();

            var reservation = new Reservation
            {
                AccountId = 123,
                CourseId = expectedCourse.CourseId,
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "Test Corp",
                Id = Guid.NewGuid(),
                IsLevyAccount = _testData.IsLevyAccount,
                ProviderId = 12345,
                StartDate = new DateTime(DateTime.Now.Year, month, 1),
                TransferSenderAccountId = null
            };

            controller.Create(reservation).Wait();
        }
        
        [Then(@"a reservation with course (.*) and start month (.*) is created")]
        public void ThenAReservationWithCourseAndStartMonthIsCreated(string courseName, string startMonth)
        {
            var month = (int) Enum.Parse<Month>(startMonth);

            var expectedCourse = _testData.Courses[courseName];

            var dbContext = _services.GetService<ReservationsDataContext>();

            var reservation = dbContext.Reservations.SingleOrDefault(r => r.CourseId == expectedCourse.CourseId);

            Assert.IsNotNull(reservation);
            Assert.IsTrue(reservation.StartDate.HasValue);
            Assert.AreEqual(month, reservation.StartDate.Value.Month);
        }
    }
}
