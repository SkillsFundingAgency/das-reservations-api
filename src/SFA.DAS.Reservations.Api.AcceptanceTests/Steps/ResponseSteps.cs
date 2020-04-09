using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class ResponseSteps : StepsBase
    {
        public ResponseSteps(
            TestData testData, 
            TestResults testResults, 
            TestServiceProvider serviceProvider) 
            : base(testData, testResults, serviceProvider)
        {
        }

        [Then("an http status code of (.*) is returned")]
        public void ThenAnHttpStatusCodeIsReturned(int httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case (int)HttpStatusCode.BadRequest:
                    var badRequestTypes = new List<Type>
                    {
                        typeof(BadRequestResult), 
                        typeof(BadRequestObjectResult)
                    };
                    badRequestTypes.Contains(TestResults.Result.GetType()).Should().BeTrue(
                        $"actual return type is {TestResults.Result.GetType()}");
                    break;
                case (int)HttpStatusCode.OK:
                    var okTypes = new List<Type>
                    {
                        typeof(OkResult), 
                        typeof(OkObjectResult)
                    };
                    okTypes.Contains(TestResults.Result.GetType()).Should().BeTrue(
                        $"actual return is {TestResults.Result}");
                    break;
                default:
                    Assert.Fail("http status code not supported");
                    break;
            }
        }
    }
}