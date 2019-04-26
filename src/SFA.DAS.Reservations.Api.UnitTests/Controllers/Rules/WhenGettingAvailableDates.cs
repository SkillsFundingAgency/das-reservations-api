using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Rules
{
    public class WhenGettingAvailableDates
    {
        private Mock<IMediator> _mediator;
        private RulesController _rulesController;
        private GetAvailableDatesResult _datesResult;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _datesResult = new GetAvailableDatesResult { AvailableDates = new List<AvailableDateStartWindow>() };
            _mediator.Setup(x => x.Send(It.IsAny<GetAvailableDatesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_datesResult);

            _rulesController = new RulesController(_mediator.Object);
        }


        [Test]
        public async Task Then_The_AvailableDates_Are_Returned()
        {
            //Act
            var actual = await _rulesController.GetAvailableDates();

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualRules = result.Value as GetAvailableDatesResult;
            Assert.AreEqual(_datesResult.AvailableDates, actualRules.AvailableDates);
        }
    }
}
