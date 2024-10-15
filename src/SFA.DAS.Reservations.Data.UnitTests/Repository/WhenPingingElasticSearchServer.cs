using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenPingingElasticSearchServer
    {
        private const string IndexName = "test-index";

        private Mock<IElasticLowLevelClient> _mockClient;
        private ReservationsApiEnvironment _apiEnvironment;
        private ReservationIndexRepository _repository;
       
        private Mock<IElasticSearchQueries> _mockElasticSearchQueries;
        
        [SetUp]
        public void Init()
        {
            _mockClient = new Mock<IElasticLowLevelClient>();

            _mockElasticSearchQueries = new Mock<IElasticSearchQueries>();
            _mockElasticSearchQueries.Setup(x => x.LastIndexSearchQuery).Returns("Get index");
            _mockElasticSearchQueries.Setup(x => x.ReservationIndexLookupName).Returns("-reservations-index-registry");

            var indexLookUpResponse =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            """ + IndexName + @""",""dateCreated"":""2019-11-06T15:11:00.5385739+00:00""},""sort"":[1573053060538]}]}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        It.IsAny<string>(),
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            _apiEnvironment = new ReservationsApiEnvironment("test");
            _repository = new ReservationIndexRepository(_mockClient.Object, _apiEnvironment, _mockElasticSearchQueries.Object, Mock.Of<ILogger<ReservationIndexRepository>>());
        }

        [Test]
        public async Task ThenReturnsTrueIfPassed()
        {
            //Arrange
            var apiCallMock = new Mock<IApiCallDetails>();
            apiCallMock.Setup(api => api.Success).Returns(true);

            _mockClient.Setup(c => c.CountAsync<StringResponse>(IndexName, It.IsAny<PostData>(), It.IsAny<CountRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse
                {
                    ApiCall = apiCallMock.Object
                });

            //Act
            var result = await _repository.PingAsync();

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task ThenReturnsFailIfFailed()
        {
            //Arrange
            var apiCallMock = new Mock<IApiCallDetails>();
            apiCallMock.Setup(api => api.Success).Returns(false);

            _mockClient.Setup(c => c.CountAsync<StringResponse>(IndexName, It.IsAny<PostData>(), It.IsAny<CountRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse
                {
                    ApiCall = apiCallMock.Object
                });

            //Act
            var result = await _repository.PingAsync();

            //Assert
            result.Should().BeFalse();
        }
    }
}
