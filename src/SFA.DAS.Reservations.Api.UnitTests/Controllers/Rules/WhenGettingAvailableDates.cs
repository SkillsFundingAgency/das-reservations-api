using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
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
        private Fixture _fixture;
        private Mock<IMediator> _mediator;
        private RulesController _rulesController;
        private GetAvailableDatesResult _datesResult;
        private long _accountLegalEntityId;
        
        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _accountLegalEntityId = _fixture.Create<long>();
            _datesResult = new GetAvailableDatesResult { AvailableDates = new List<AvailableDateStartWindow>() };
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<GetAvailableDatesQuery>(query => query.AccountLegalEntityId == _accountLegalEntityId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_datesResult);

            _rulesController = new RulesController(_mediator.Object);
        }

        [Test]
        public async Task Then_The_AvailableDates_Are_Returned()
        {
            //Act
            var actual = await _rulesController.GetAvailableDates(_accountLegalEntityId);

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
