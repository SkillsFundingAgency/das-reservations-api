using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Constants;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AzureSearchReservationIndexRepositoryTests;

[TestFixture]
public class WhenGettingHealthCheckStatus
{
    private AzureSearchReservationIndexRepository _repository;
    private Mock<IAzureSearchHelper> _mockSearchHelper;
    private Mock<ILogger<AzureSearchReservationIndexRepository>> _mockLogger;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void Setup()
    {
        _mockSearchHelper = new Mock<IAzureSearchHelper>();
        _mockLogger = new Mock<ILogger<AzureSearchReservationIndexRepository>>();
        _cancellationToken = CancellationToken.None;

        _repository = new AzureSearchReservationIndexRepository(
            _mockSearchHelper.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task Then_Returns_Healthy_When_Index_Is_Fresh()
    {
        // Arrange
        var freshDate = DateTime.UtcNow.AddMinutes(-30);
        var indexName = $"{AzureSearchIndex.IndexName}-{freshDate:yyyyMMddHHmmss}";

        _mockSearchHelper
            .Setup(x => x.GetIndexName(_cancellationToken))
            .ReturnsAsync(indexName);

        // Act
        var result = await _repository.GetHealthCheckStatus(_cancellationToken);

        // Assert
        result.Should().Be(HealthCheckResult.Healthy);
    }

    [Test]
    public async Task Then_Returns_Degraded_When_Index_Is_Stale()
    {
        // Arrange
        var staleDate = DateTime.UtcNow.AddHours(-2);
        var indexName = $"{AzureSearchIndex.IndexName}-{staleDate:yyyyMMddHHmmss}";

        _mockSearchHelper
            .Setup(x => x.GetIndexName(_cancellationToken))
            .ReturnsAsync(indexName);

        // Act
        var result = await _repository.GetHealthCheckStatus(_cancellationToken);

        // Assert
        result.Should().Be(HealthCheckResult.Degraded);
    }

    [Test]
    public async Task Then_Returns_Degraded_When_Exception_Occurs()
    {
        // Arrange
        _mockSearchHelper
            .Setup(x => x.GetIndexName(_cancellationToken))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _repository.GetHealthCheckStatus(_cancellationToken);

        // Assert
        result.Should().Be(HealthCheckResult.Degraded);
    }
}