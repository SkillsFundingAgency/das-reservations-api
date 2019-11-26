using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
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

            _apiEnvironment = new ReservationsApiEnvironment("test");
            _repository = new ReservationIndexRepository(_mockClient.Object, _apiEnvironment, _mockElasticSearchQueries.Object, Mock.Of<ILogger<ReservationIndexRepository>>());
        }

        [Test]
        public async Task ThenReturnsTrueIfPassed()
        {
            //Arrange
            _mockClient.Setup(c => c.PingAsync<StringResponse>(It.IsAny<PingRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse
                {
                    ApiCall = new ApiCallDetails {HttpStatusCode = 200, Success = true}
                });

            //Act
            var result = await _repository.PingAsync();

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ThenReturnsFailIfFailed()
        {
            //Arrange
            _mockClient.Setup(c => c.PingAsync<StringResponse>(It.IsAny<PingRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse
                {
                    ApiCall = new ApiCallDetails {HttpStatusCode = 400, Success = false}
                });

            //Act
            var result = await _repository.PingAsync();

            //Assert
            Assert.IsFalse(result);
        }
    }
}
