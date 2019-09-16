using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.HealthCheck;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.HealthCheck
{
    public class WhenHandlingQueueMonitoring
    {
        [Test, MoqAutoData]
        public async Task Then_The_Queue_Details_Are_Retrieved(
            [Frozen] Mock<IAzureQueueService> azureQueueService,
            HealthCheckContext healthCheckContext,
            QueueHealthCheck handler)
        {
            //Arrange
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).Returns(new List<QueueMonitor>
            {
                new QueueMonitor("test.queue", false, "LOCAL")
            });

            //Act
            await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            azureQueueService.Verify(x => x.GetQueuesToMonitor(), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Each_Queue_Is_Checked_And_The_Status_Returned(
            [Frozen] Mock<IAzureQueueService> azureQueueService,
            HealthCheckContext healthCheckContext,
            QueueHealthCheck handler)
        {
            //Arrange
            var expectedQueueName = "test.queue";
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).Returns(new List<QueueMonitor>
            {
                new QueueMonitor(expectedQueueName, false, "LOCAL")
            });

            //Act
            await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Is_Healthy_Then_A_Healthy_Response_Is_Returned(
            [Frozen] Mock<IAzureQueueService> azureQueueService,
            HealthCheckContext healthCheckContext,
            QueueHealthCheck handler
        )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName, false, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).Returns(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(true);

            //Act
            var actual = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            Assert.AreEqual(HealthStatus.Healthy, actual.Status);
            Assert.IsFalse(actual.Data.ContainsKey("QueuesInError"));
        }


        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Changed_To_Error_On_Some_Of_The_Queues_Then_A_Degraded_Response_Is_Returned_With_The_Queues_Listed(
            [Frozen] Mock<IAzureQueueService> azureQueueService,
            HealthCheckContext healthCheckContext,
            QueueHealthCheck handler
        )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var expectedQueueName2 = "test.queue2";
            var expectedQueueName3 = "test.queue3";
            var queueMonitor = new QueueMonitor(expectedQueueName, true, "LOCAL");
            var queueMonitor2 = new QueueMonitor(expectedQueueName2, true, "LOCAL");
            var queueMonitor3 = new QueueMonitor(expectedQueueName3, true, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).Returns(new List<QueueMonitor>
            {
                queueMonitor,
                queueMonitor2,
                queueMonitor3
            });
            azureQueueService.SetupSequence(x => x.IsQueueHealthy(It.IsAny<string>()))
                .ReturnsAsync(false).ReturnsAsync(false).ReturnsAsync(true);

            //Act
            var actual = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            Assert.AreEqual(HealthStatus.Degraded, actual.Status);
            Assert.IsTrue(actual.Data.ContainsKey("QueuesInError"));
            Assert.AreEqual("test.queue, test.queue2", actual.Data["QueuesInError"]);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Changed_To_Error_On_All_Of_The_Queues_Then_A_Unhealthy_Response_Is_Returned_With_The_Queues_Listed(
                [Frozen] Mock<IAzureQueueService> azureQueueService,
                HealthCheckContext healthCheckContext,
                QueueHealthCheck handler
            )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var expectedQueueName2 = "test.queue2";
            var expectedQueueName3 = "test.queue3";
            var queueMonitor = new QueueMonitor(expectedQueueName, true, "LOCAL");
            var queueMonitor2 = new QueueMonitor(expectedQueueName2, true, "LOCAL");
            var queueMonitor3 = new QueueMonitor(expectedQueueName3, true, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).Returns(new List<QueueMonitor>
            {
                queueMonitor,
                queueMonitor2,
                queueMonitor3
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(It.IsAny<string>())).ReturnsAsync(false);

            //Act
            var actual = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            Assert.AreEqual(HealthStatus.Unhealthy, actual.Status);
            Assert.IsTrue(actual.Data.ContainsKey("QueuesInError"));
            Assert.AreEqual("test.queue, test.queue2, test.queue3", actual.Data["QueuesInError"]);
        }


        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Not_Changed_And_Is_UnHealthy_A_UnHealthy_Response_Is_Returned(
            [Frozen] Mock<IAzureQueueService> azureQueueService,
            HealthCheckContext healthCheckContext,
            QueueHealthCheck handler
        )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName, false, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).Returns(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(false);

            //Act
            var actual = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            Assert.AreEqual(HealthStatus.Unhealthy, actual.Status);
        }


        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Not_Been_Set_And_Unhealthy_A_Unhealthy_Response_Is_Returned(
            [Frozen] Mock<IAzureQueueService> azureQueueService,
            HealthCheckContext healthCheckContext,
            QueueHealthCheck handler
        )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName, null, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).Returns(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(false);

            //Act
            var actual = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            Assert.AreEqual(HealthStatus.Unhealthy, actual.Status);
        }

    }
}
