using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Elasticsearch.Net;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.HealthCheck;
using SFA.DAS.Reservations.Infrastructure.UnitTests.Extensions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.HealthCheck
{
    public class WhenHandlingElasticSearchMonitoring
    {
        [Test, MoqAutoData]
        public async Task Then_Checks_To_See_If_A_Connection_Can_Be_Made(
            [Frozen] Mock<IElasticLowLevelClient> client,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Act
            await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            client.Verify(c => c.PingAsync<StringResponse>(It.IsAny<PingRequestParameters>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Checks_To_See_If_The_How_Old_The_Latest_Index_Is(
            [Frozen] Mock<IElasticLowLevelClient> client,
            [Frozen] Mock<ReservationsApiEnvironment> environment,
            [Frozen] Mock<IElasticSearchQueries> elasticSearchQueries,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            var environmentName = "test";
            var indexLookupName = "-lookup";
            var expectedQueryString = "test query";

            environment.Setup(e => e.EnvironmentName).Returns(environmentName);
            elasticSearchQueries.Setup(esq => esq.ReservationIndexLookupName).Returns(indexLookupName);
            elasticSearchQueries.Setup(esq => esq.LastIndexSearchQuery).Returns(expectedQueryString);

            //Act
            await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            client.Verify(c => c.SearchAsync<StringResponse>(
                $"{environmentName}{indexLookupName}", 
                It.Is<PostData>(pd => pd.GetRequestString().Equals(expectedQueryString)), 
                It.IsAny<SearchRequestParameters>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Healthy_Status_If_All_Checks_Pass(
            [Frozen] Mock<IElasticLowLevelClient> client,
            [Frozen] Mock<ReservationsApiEnvironment> environment,
            [Frozen] Mock<IElasticSearchQueries> elasticSearchQueries,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            var environmentName = "test";
            var indexLookupName = "-lookup";
            var expectedQueryString = "test query";

            var indexLookUpResponse =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            ""test_index"",""dateCreated"":""" + DateTime.Now.ToLongDateString() + @"""},""sort"":[1573053060538]}]}}";

            environment.Setup(e => e.EnvironmentName).Returns(environmentName);
            elasticSearchQueries.Setup(esq => esq.ReservationIndexLookupName).Returns(indexLookupName);
            elasticSearchQueries.Setup(esq => esq.LastIndexSearchQuery).Returns(expectedQueryString);

            client.Setup(c => c.PingAsync<StringResponse>(It.IsAny<PingRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(""));

            client.Setup(c => c.SearchAsync<StringResponse>(
                It.IsAny<string>(), 
                It.IsAny<PostData>(), 
                It.IsAny<SearchRequestParameters>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            //Act
            var result = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
        }

         [Test, MoqAutoData]
        public async Task Then_Returns_UnHealthy_Status_If_Elastic_Cannot_Be_Reached(
            [Frozen] Mock<IElasticLowLevelClient> client,
            [Frozen] Mock<ReservationsApiEnvironment> environment,
            [Frozen] Mock<IElasticSearchQueries> elasticSearchQueries,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            var environmentName = "test";
            var indexLookupName = "-lookup";
            var expectedQueryString = "test query";

            environment.Setup(e => e.EnvironmentName).Returns(environmentName);
            elasticSearchQueries.Setup(esq => esq.ReservationIndexLookupName).Returns(indexLookupName);
            elasticSearchQueries.Setup(esq => esq.LastIndexSearchQuery).Returns(expectedQueryString);

            client.Setup(c => c.PingAsync<StringResponse>(It.IsAny<PingRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse
                {
                    ApiCall = new ApiCallDetails {HttpStatusCode = 400}
                });

            //Act
            var result = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);

            client.Verify(c => c.SearchAsync<StringResponse>(
                It.IsAny<string>(), 
                It.IsAny<PostData>(), 
                It.IsAny<SearchRequestParameters>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Degraded_Status_If_Latest_Index_Is_Too_Old(
            [Frozen] Mock<IElasticLowLevelClient> client,
            [Frozen] Mock<ReservationsApiEnvironment> environment,
            [Frozen] Mock<IElasticSearchQueries> elasticSearchQueries,
            HealthCheckContext healthCheckContext,
            ElasticSearchHealthCheck handler)
        {
            //Arrange
            var environmentName = "test";
            var indexLookupName = "-lookup";
            var expectedQueryString = "test query";

            var indexLookUpResponse =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            ""test_index"",""dateCreated"":""" + DateTime.Now.AddHours(-25).ToLongDateString() + @"""},""sort"":[1573053060538]}]}}";

            environment.Setup(e => e.EnvironmentName).Returns(environmentName);
            elasticSearchQueries.Setup(esq => esq.ReservationIndexLookupName).Returns(indexLookupName);
            elasticSearchQueries.Setup(esq => esq.LastIndexSearchQuery).Returns(expectedQueryString);

            client.Setup(c => c.PingAsync<StringResponse>(It.IsAny<PingRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(""));

            client.Setup(c => c.SearchAsync<StringResponse>(
                It.IsAny<string>(), 
                It.IsAny<PostData>(), 
                It.IsAny<SearchRequestParameters>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            //Act
            var result = await handler.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Degraded, result.Status);
        }
        
    }
}
