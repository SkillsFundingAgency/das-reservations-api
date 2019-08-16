using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;
using Course = SFA.DAS.Reservations.Domain.Entities.Course;
using Reservation = SFA.DAS.Reservations.Api.Models.Reservation;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    [Binding]
    public class CreateReservationSteps
    {
        private const string CourseId = "1";
        private readonly TestContainer _container;

        public CreateReservationSteps()
        {
            _container = TestContainer.GetInstance();
        }

        [Given(@"a course name (.*) has been added to the course list")]
        public void GivenACourseNameHasBeenAddedToTheCourseList(string courseName)
        {
            var dbContext = _container.Get<ReservationsDataContext>();

            dbContext.Courses.Add(new Course
            {
                CourseId = CourseId,
                Level = 1,
                Title = courseName
            });

            dbContext.AccountLegalEntities.Add(new AccountLegalEntity
            {
                AccountLegalEntityId = 1,
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                AgreementSigned = true
            });

            dbContext.SaveChanges();
        }
        
        [When(@"I create a reservation for the (.*) course with a start date of (.*)")]
        public void WhenICreateAReservationForTheCourseWithAStartDate(string courseName, string startMonth)
        {
            var controller = _container.Get<ReservationsController>();

            var reservation = new Reservation
            {
                AccountId = 123,
                CourseId = CourseId,
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "Test Corp",
                Id = Guid.NewGuid(),
                IsLevyAccount = false,
                ProviderId = 12345,
                StartDate = DateTime.Now,
                TransferSenderAccountId = null
            };

            controller.Create(reservation).Wait();
        }
        
        [Then(@"a reservation with course (.*) and start date (.*) is created")]
        public void ThenAReservationWithCourseAndStartDateIsCreated(string courseName, string startMonth)
        {
            var dbContext = _container.Get<ReservationsDataContext>();

            var reservation = dbContext.Reservations.SingleOrDefault(r => r.CourseId == CourseId);

            Assert.IsNotNull(reservation);
        }
    }
}
