using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Data;
using TechTalk.SpecFlow;
using Reservation = SFA.DAS.Reservations.Api.Models.Reservation;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    
    

    [Binding]
    public class Steps : StepsBase
    {
        public Steps(TestData testData, TestServiceProvider serviceProvider) : base(testData, serviceProvider)
        {
        }


        [Given(@"I have a non levy account")]
        public void GivenIHaveANonLevyAccount()
        {
            TestData.IsLevyAccount = false;
        }

        [Given(@"I have a levy account")]
        public void GivenIHaveALevyAccount()
        {
            TestData.IsLevyAccount = true;
        }

        [Given(@"it has a reservation limit of (.*)")]
        public void GivenItHasAReservationLimitOf(int limit)
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var accountLegalEntity = dbContext.AccountLegalEntities.FirstOrDefault(c=>c.AccountLegalEntityId.Equals(1));
            accountLegalEntity.ReservationLimit = limit;
            dbContext.SaveChanges();
        }

        [Given(@"I have an existing reservation")]
        public void GivenIHaveAnExistingReservation()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            dbContext.Reservations.Add(new Domain.Entities.Reservation
            {
                AccountId = 1,
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "Test",
                CourseId = "234",
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(2),
                IsLevyAccount = false,
                Status = 1,
                StartDate = DateTime.UtcNow.AddMonths(1),
                Id = Guid.NewGuid()
            });
            dbContext.SaveChanges();
        }


        [When(@"I create a reservation for a course with a start month of (.*)")]
        public void WhenICreateAReservationForACourseWithAStartMonth(string startMonth)
        {
            var month = (int) Enum.Parse<Month>(startMonth);
           
            var expectedCourse = TestData.Course;

            var controller = Services.GetService<ReservationsController>();

            var reservation = new Reservation
            {
                AccountId = 1,
                CourseId = expectedCourse.CourseId,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                Id = Guid.NewGuid(),
                IsLevyAccount = TestData.IsLevyAccount,
                ProviderId = 12345,
                StartDate = new DateTime(DateTime.Now.Year, month, 1),
                TransferSenderAccountId = null
            };

            controller.Create(reservation).Wait();
        }

        [When(@"I create a levy reservation for a course with a start month of (.*)")]
        public void WhenICreateALevyReservationForACourseWithAStartMonthOfJuly(string startMonth)
        {
            var month = (int)Enum.Parse<Month>(startMonth);

            var expectedCourse = TestData.Course;

            var controller = Services.GetService<ReservationsController>();

            var reservation = new Reservation
            {
                AccountId = 1,
                CourseId = expectedCourse.CourseId,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                Id = Guid.NewGuid(),
                IsLevyAccount = true,
                ProviderId = 12345,
                StartDate = new DateTime(DateTime.Now.Year, month, 1),
                TransferSenderAccountId = null
            };

            controller.Create(reservation).Wait();
        }


        [Then(@"a reservation with course and start month (.*) is created")]
        public void ThenAReservationWithCourseAndStartMonthIsCreated(string startMonth)
        {
            var month = (int) Enum.Parse<Month>(startMonth);

            var expectedCourse = TestData.Course;

            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = dbContext.Reservations.SingleOrDefault(r => r.CourseId == expectedCourse.CourseId && 
                                                                          r.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNotNull(reservation);
            Assert.IsTrue(reservation.StartDate.HasValue);
            Assert.AreEqual(month, reservation.StartDate.Value.Month);
            Assert.AreEqual(TestData.IsLevyAccount, reservation.IsLevyAccount);
        }

        [Then(@"I have (.*) reservation")]
        public void ThenIHaveReservation(int reservationCount)
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = dbContext.Reservations.Count();

            Assert.AreEqual(reservationCount,reservation);
        }

    }
}
