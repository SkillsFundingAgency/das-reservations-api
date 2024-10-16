using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.ProviderPermissions.Queries;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.AccountLegalEntities;

public class WhenGettingAccountLegalEntitiesForProvider
{
    private AccountLegalEntitiesController _accountLegalEntitiesController;
    private Mock<IMediator> _mediator;
    private GetAccountLegalEntitiesForProviderResponse _accountLegalEntitiesResponse;
    private const uint ExpectedProviderId = 123234;

    [SetUp]
    public void Arrange()
    {
        _mediator = new Mock<IMediator>();
        _accountLegalEntitiesResponse = new GetAccountLegalEntitiesForProviderResponse
            { ProviderPermissions = new List<Domain.ProviderPermissions.ProviderPermission>() };

        _mediator.Setup(x =>
                x.Send(
                    It.Is<GetAccountLegalEntitiesForProviderQuery>(c =>
                        c.ProviderId.Equals(ExpectedProviderId)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_accountLegalEntitiesResponse);

        _accountLegalEntitiesController = new AccountLegalEntitiesController(_mediator.Object,
            Mock.Of<ILogger<AccountLegalEntitiesController>>());
    }

    [Test]
    public async Task Then_The_AccountLegalEntities_Are_Returned()
    {
        //Act
        var actual = await _accountLegalEntitiesController.GetByProviderId(ExpectedProviderId);

        //Assert
        actual.Should().NotBeNull();

        var result = actual.Should().BeOfType<ObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().NotBeNull();

        var actualAccountLegalEntities = result.Value.Should()
            .BeOfType<List<Domain.ProviderPermissions.ProviderPermission>>().Subject;
        actualAccountLegalEntities.Should().BeEquivalentTo(_accountLegalEntitiesResponse.ProviderPermissions);
    }

    [Test]
    public async Task Then_If_A_Validation_Error_Occurs_A_Bad_Request_Is_Returned_With_Errors()
    {
        //Arrange
        var expectedValidationMessage = "The following parameters have failed validation";
        var expectedParam = "ProviderId";
        _mediator.Setup(x =>
                x.Send(It.IsAny<GetAccountLegalEntitiesForProviderQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

        //Act
        var actual = await _accountLegalEntitiesController.GetByProviderId(0);

        //Assert
        var result = actual.Should().BeOfType<ObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        var actualError = result.Value.Should().BeOfType<ArgumentErrorViewModel>().Subject;
        actualError.Message.Should().Be($"{expectedValidationMessage} (Parameter '{expectedParam}')");
        actualError.Params.Should().Be(expectedParam);
    }
}