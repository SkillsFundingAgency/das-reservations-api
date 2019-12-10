using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Data;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Rules;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class AvailableDatesSteps: StepsBase
    {
        private const int DefaultAvailableDatesMonthCount = 6;
        private GetAvailableDatesResult _availableDatesResult;

        public AvailableDatesSteps(TestData testData, TestServiceProvider serviceProvider) : base(testData, serviceProvider)
        {
        }

        [Given(@"I have signed an Agreement")]
        public void GivenIHaveSignedAnAgreement()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var legalEntity = dbContext.AccountLegalEntities.Single(le => le.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            legalEntity.AgreementType = AgreementType.Levy;

            dbContext.SaveChanges();
        }
        
        [When(@"I get available dates")]
        public void WhenIGetAvailableDates()
        {
            var controller = Services.GetService<RulesController>();

            var result = controller.GetAvailableDates(TestData.AccountLegalEntity.AccountLegalEntityId).Result as OkObjectResult;

            _availableDatesResult = result?.Value as GetAvailableDatesResult;
        }
        
        [Then(@"I should get available dates back")]
        public void ThenIShouldGetStandardAvailableDatesBack()
        {
            Assert.AreEqual(6, _availableDatesResult.AvailableDates.Count());

            var expectedDates = GetExpectedDates(DefaultAvailableDatesMonthCount, DateTime.Now,  DateTime.MaxValue);

            _availableDatesResult.AvailableDates.Should().BeEquivalentTo(expectedDates);
        }

        private static IEnumerable<AvailableDateStartWindow> GetExpectedDates(int expectedMonthCount, DateTime minDate, DateTime maxDate)
        {
            var expectedDates = new List<AvailableDateStartWindow>();

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
