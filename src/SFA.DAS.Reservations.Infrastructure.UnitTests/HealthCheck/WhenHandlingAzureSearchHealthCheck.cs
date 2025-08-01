using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure.HealthCheck;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.HealthCheck;

[TestFixture]
public class WhenHandlingAzureSearchHealthCheck
{
    private Mock<IAzureSearchReservationIndexRepository> _mockRepository;
    private AzureSearchHealthCheck _healthCheck;
    private HealthCheckContext _context;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IAzureSearchReservationIndexRepository>();
        _healthCheck = new AzureSearchHealthCheck(_mockRepository.Object);
        _context = new HealthCheckContext();
        _cancellationToken = new CancellationToken();
    }

    [Test]
    public async Task CheckHealthAsync_WhenRepositoryReturnsHealthy_ThenReturnsHealthy()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetHealthCheckStatus(_cancellationToken))
            .ReturnsAsync(Domain.Models.HealthCheckResult.Healthy);

        // Act
        var result = await _healthCheck.CheckHealthAsync(_context, _cancellationToken);

        // Assert
        result.Status.Should().Be(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy);
        result.Description.Should().Be("Azure search re-indexing health");
    }

    [Test]
    public async Task CheckHealthAsync_WhenRepositoryReturnsUnHealthy_ThenReturnsDegraded()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetHealthCheckStatus(_cancellationToken))
            .ReturnsAsync(Domain.Models.HealthCheckResult.UnHealthy);

        // Act
        var result = await _healthCheck.CheckHealthAsync(_context, _cancellationToken);

        // Assert
        result.Status.Should().Be(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
        result.Description.Should().Be("Azure search re-indexing health");
    }

    [Test]
    public async Task CheckHealthAsync_WhenRepositoryReturnsDegraded_ThenReturnsDegraded()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetHealthCheckStatus(_cancellationToken))
            .ReturnsAsync(Domain.Models.HealthCheckResult.Degraded);

        // Act
        var result = await _healthCheck.CheckHealthAsync(_context, _cancellationToken);

        // Assert
        result.Status.Should().Be(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
        result.Description.Should().Be("Azure search re-indexing health");
    }   
}