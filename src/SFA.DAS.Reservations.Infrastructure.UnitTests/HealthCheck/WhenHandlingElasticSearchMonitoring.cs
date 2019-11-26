using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure.HealthCheck;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.HealthCheck
{
    public class WhenHandlingElasticSearchMonitoring
    {
        [Test, MoqAutoData]
        public async Task Then_Checks_To_See_If_A_Connection_Can_Be_Made(
            [Frozen] Mock<IReservationIndexRepository> repository,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Act
            await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            repository.Verify(c => c.PingAsync(), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Checks_To_See_If_The_How_Old_The_Latest_Index_Is(
            [Frozen] Mock<IReservationIndexRepository> repository,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Act
            await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            repository.Verify(r => r.GetCurrentReservationIndex(), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Healthy_Status_If_All_Checks_Pass(
            [Frozen] Mock<IReservationIndexRepository> repository,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            repository.Setup(r => r.PingAsync()).ReturnsAsync(true);
            repository.Setup(r => r.GetCurrentReservationIndex()).ReturnsAsync(new IndexRegistryEntry
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                DateCreated = DateTime.Now.AddHours(-1)
            });

            //Act
            var result = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
        }

         [Test, MoqAutoData]
        public async Task Then_Returns_UnHealthy_Status_If_Elastic_Cannot_Be_Reached(
            [Frozen] Mock<IReservationIndexRepository> repository,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            repository.Setup(r => r.PingAsync()).ReturnsAsync(false);

            //Act
            var result = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);

            repository.Verify(r => r.GetCurrentReservationIndex(), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Degraded_Status_If_Latest_Index_Is_More_Than_One_Day_Old(
            [Frozen] Mock<IReservationIndexRepository> repository,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            repository.Setup(r => r.PingAsync()).ReturnsAsync(true);
            repository.Setup(r => r.GetCurrentReservationIndex()).ReturnsAsync(new IndexRegistryEntry
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                DateCreated = DateTime.Now.AddHours(-25)
            });

            //Act
            var result = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Degraded, result.Status);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_UnHealthy_Status_If_There_Is_No_Indices(
            [Frozen] Mock<IReservationIndexRepository> repository,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            repository.Setup(r => r.PingAsync()).ReturnsAsync(true);
            repository.Setup(r => r.GetCurrentReservationIndex()).ReturnsAsync((IndexRegistryEntry) null);

            //Act
            var result = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
        }
        
    }
}
