﻿using System;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Reservation = SFA.DAS.Reservations.Api.Models.Reservation;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class CreateReservationSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider)
        : StepsBase(testData, testResults, serviceProvider)
    {
        [Given(@"I have a non levy account")]
        public void GivenIHaveANonLevyAccount()
        {
            SetAccountIsLevyFlag(false);
        }

        [Given(@"I have a levy account")]
        public void GivenIHaveALevyAccount()
        {
            SetAccountIsLevyFlag(true);
        }

        private void SetAccountIsLevyFlag(bool entityIsLevy)
        {
            var context = Services.GetService<ReservationsDataContext>();

            foreach (var entity in context.Accounts)
            {
                entity.IsLevy = entityIsLevy;
            }

            context.SaveChanges();

            TestData.IsLevyAccount = entityIsLevy;
        }

        [Given(@"it has a reservation limit of (.*)")]
        public void GivenItHasAReservationLimitOf(int limit)
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var accountLegalEntity = dbContext.AccountLegalEntities.FirstOrDefault(c=>c.AccountLegalEntityId.Equals(1));
            accountLegalEntity.Account.ReservationLimit = limit;
            dbContext.SaveChanges();
        }

        [Given(@"I have an existing reservation with status (.*)")]
        public void GivenIHaveAnExistingReservationWithStatus(string requiredStatus)
        {
            TestData.ReservationId = Guid.NewGuid();

            Enum.TryParse(requiredStatus, true, out ReservationStatus status);

            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = new Domain.Entities.Reservation
            {
                AccountId = 1,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                CourseId = TestData.Course.CourseId,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(2),
                IsLevyAccount = false,
                Status = (short) status,
                StartDate = DateTime.UtcNow.AddMonths(1),
                Id = TestData.ReservationId,
                UserId = TestData.UserId
            };

            dbContext.Reservations.Add(reservation);
            dbContext.SaveChanges();
        }

        [Given(@"I have reservation before reservation reset date")]
        public void GivenIHaveAnExistingReservationWithStatusBeforeReservationResetDate()
        {
            TestData.ReservationId = Guid.NewGuid();

            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = new Domain.Entities.Reservation
            {
                AccountId = 1,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                CourseId = TestData.Course.CourseId,
                CreatedDate = DateTime.UtcNow.AddMonths(-1),
                ExpiryDate = DateTime.UtcNow.AddMonths(2),
                IsLevyAccount = false,
                Status = (short)ReservationStatus.Completed,
                StartDate = DateTime.UtcNow.AddMonths(1),
                Id = TestData.ReservationId,
                UserId = TestData.UserId
            };

            dbContext.Reservations.Add(reservation);
            dbContext.SaveChanges();
        }

        [Given(@"I have the following existing reservation:")]
        public void GivenIHaveTheFollowingExistingReservation(Table table)
        {
            TestData.ReservationId = Guid.NewGuid();
            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = table.CreateInstance(() => new Domain.Entities.Reservation
            {
                Id = TestData.ReservationId,
                AccountId = 1,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                CourseId = TestData.Course.CourseId,
                CreatedDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow.Date.AddMonths(-1),
                ExpiryDate = DateTime.UtcNow.Date.AddMonths(2),
                IsLevyAccount = false,
                Status = (short) ReservationStatus.Pending,
                UserId = TestData.UserId
            });

            dbContext.Reservations.Add(reservation);
            dbContext.SaveChanges();
        }
        
        [When(@"I create a reservation for a course with a start month of (.*)")]
        public void WhenICreateAReservationForACourseWithAStartMonth(string startMonth)
        {
            TestData.ReservationId = Guid.NewGuid();
            var month = (int) Enum.Parse<Month>(startMonth);

            var controller = Services.GetService<ReservationsController>();

            var reservation = new Reservation
            {
                AccountId = 1,
                CourseId = TestData.Course.CourseId,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                Id = TestData.ReservationId,
                IsLevyAccount = TestData.IsLevyAccount,
                ProviderId = 12345,
                StartDate = new DateTime(DateTime.Now.Year, month, 1),
                TransferSenderAccountId = null,
                UserId = TestData.UserId
            };

            TestResults.Result = (controller.Create(reservation).Result) as CreatedResult;
        }

        [When(@"I create a levy reservation")]
        public void WhenICreateALevyReservationForACourseWithAStartMonthOfJuly()
        {
            TestData.ReservationId = Guid.NewGuid();
            var controller = Services.GetService<ReservationsController>();
            var reservation = new Reservation
            {
                AccountId = 1,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                Id = TestData.ReservationId,
                IsLevyAccount = true,
                ProviderId = 12345,
                TransferSenderAccountId = null,
                UserId = TestData.UserId
            };

            TestResults.Result = controller.Create(reservation).Result as CreatedResult;
            
        }


        [Then(@"a reservation with course and start month (.*) is created")]
        public void ThenAReservationWithCourseAndStartMonthIsCreated(string startMonth)
        {
            var month = (int) Enum.Parse<Month>(startMonth);
           
            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = dbContext.Reservations.SingleOrDefault(r => r.CourseId == TestData.Course.CourseId && 
                                                                          r.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            reservation.Should().NotBeNull();
            reservation.StartDate.Should().HaveValue();
            reservation.StartDate.Value.Month.Should().Be(month);
            reservation.IsLevyAccount.Should().Be(TestData.IsLevyAccount);
            reservation.UserId.Should().Be(TestData.UserId);
            var actual = TestResults.Result as CreatedResult;
            actual.Should().NotBeNull();
            actual.Location.Should().Be($"api/Reservations/{TestData.ReservationId}");
        }

        [Then(@"I have (.*) reservation")]
        public void ThenIHaveReservation(int reservationCount)
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = dbContext.Reservations.Count();

            reservation.Should().Be(reservationCount);
            
        }

        [Then(@"I have the following reservations:")]
        public void ThenIHaveTheFollowingReservations(Table table)
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservations = dbContext.Reservations.ToList();

            table.CompareToSet(reservations);
        }
    }
}
