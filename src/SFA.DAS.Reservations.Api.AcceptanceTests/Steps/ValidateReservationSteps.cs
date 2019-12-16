using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class ValidateReservationSteps : StepsBase
    {
        private IActionResult _validationResult;

        public ValidateReservationSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider) : base(testData, testResults, serviceProvider)
        {
        }

        // Given

        [Given(@"the following rule exists:")]
        public void GivenTheFollowingRuleExists(Table table)
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var rule = new Domain.Entities.Rule
            {
                CourseId = TestData.Course.CourseId,
                CreatedDate = DateTime.UtcNow.Date.AddMonths(-1),
                ActiveFrom = DateTime.UtcNow.Date.AddMonths(-1),
                ActiveTo = DateTime.UtcNow.Date.AddMonths(2),
                Restriction = 0
            };
            table.FillInstance(rule);

            dbContext.Rules.Add(rule);
            dbContext.SaveChanges();
        }

        // When

        [When(@"I validate the reservation against the following commitment data:")]
        public void WhenIValidateTheReservationAgainstTheFollowingCommitmentData(Table table)
        {
            var controller = Services.GetService<ReservationsController>();
            var validationArguments = table.CreateInstance<ValidationArguments>();

            _validationResult = controller.Validate(
                TestData.ReservationId, 
                validationArguments.CourseId,
                validationArguments.StartDate).Result;
        }

        // Then

        [Then(@"no validation errors are returned")]
        public void ThenNoValidationErrorsAreReturned()
        {
            var result = _validationResult as OkObjectResult;
            var model = result.Value as ValidateReservationViewModel;

            model.ValidationErrors.Should().BeEmpty();
        }
        
        [Then(@"validation errors are returned")]
        public void ThenValidationErrorsAreReturned()
        {
            var result = _validationResult as OkObjectResult;
            var model = result.Value as ValidateReservationViewModel;

            model.ValidationErrors.Should().NotBeEmpty();
        }
    }

    public class ValidationArguments
    {
        public string CourseId { get; set; }
        public DateTime StartDate { get; set; }
    }
}