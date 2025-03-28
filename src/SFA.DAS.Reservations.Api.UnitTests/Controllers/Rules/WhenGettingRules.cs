﻿using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Rules
{
    public class WhenGettingRules
    {
        private Mock<IMediator> _mediator;
        private RulesController _rulesController;
        private GetRulesResult _rulesResult;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _rulesResult = new GetRulesResult{Rules = new List<ReservationRule>()};
            _mediator.Setup(x => x.Send(It.IsAny<GetRulesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_rulesResult);

            _rulesController = new RulesController(_mediator.Object);
        }

        [Test]
        public async Task Then_The_Rules_Are_Returned()
        {
            //Act
            var actual = await _rulesController.All();

            //Assert
            actual.Should().NotBeNull();

            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();

            var actualRules = result.Value.Should().BeAssignableTo<GetRulesResult>().Subject;
            actualRules.Rules.Should().BeEquivalentTo(_rulesResult.Rules);

        }

        [Test]
        public async Task Then_The_Global_Rules_Are_Returned()
        {
            //Act
            var actual = await _rulesController.All();

            //Assert
            actual.Should().NotBeNull();

            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();

            var actualRules = result.Value.Should().BeAssignableTo<GetRulesResult>().Subject;
            actualRules.GlobalRules.Should().BeEquivalentTo(_rulesResult.GlobalRules);

        }
    }
}
