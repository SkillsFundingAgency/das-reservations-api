using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Application.Rules.Queries.GetAvailableDates;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Rules;

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
        actual.Should().NotBeNull();

        var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().NotBeNull();

        var actualRules = result.Value.Should().BeAssignableTo<GetAvailableDatesResult>().Subject;
        actualRules.AvailableDates.Should().BeEquivalentTo(_datesResult.AvailableDates);

    }

    [Test, AutoData]
    public async Task And_EntityNotFoundException_Then_Returns_Error(
        EntityNotFoundException<AccountLegalEntity> notFoundException)
    {
        _mediator.Setup(x => x.Send(
                It.IsAny<GetAvailableDatesQuery>(),
                It.IsAny<CancellationToken>()))
            .Throws(notFoundException);

        var actual = await _rulesController.GetAvailableDates(_accountLegalEntityId) as ObjectResult;
        actual.Should().NotBeNull();
        actual.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        var actualError = actual.Value.Should().BeOfType<ArgumentErrorViewModel>().Subject;
        actualError.Message.Should().Be(notFoundException.Message);
        actualError.Params.Should().Be("accountLegalEntityId");

    }

    [Test, AutoData]
    public async Task And_ArgumentException_Then_Returns_Error(
        ArgumentException argumentException)
    {
        // Arrange
        _mediator.Setup(x => x.Send(
                It.IsAny<GetAvailableDatesQuery>(),
                It.IsAny<CancellationToken>()))
            .Throws(argumentException);

        // Act
        var actual = await _rulesController.GetAvailableDates(_accountLegalEntityId) as ObjectResult;
            
        // Assert
        actual.Should().NotBeNull();
        actual.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        var actualError = actual.Value.Should().BeOfType<ArgumentErrorViewModel>().Subject;
        actualError.Message.Should().Be(argumentException.Message);
        actualError.Params.Should().Be("accountLegalEntityId");

    }
}