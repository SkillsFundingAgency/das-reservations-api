using System;
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
    public class WhenGettingCurrentReservationIndex
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
        public async Task ThenWillSearchElasticForLatest()
        {
            //Arrange
            var indexLookUpResponse = @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            ""test"",""dateCreated"":""2019-11-06T15:00:00+00:00""},""sort"":[1573053060538]}]}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        "test-reservations-index-registry",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            //Act
            var index = await _repository.GetCurrentReservationIndex();

            //Assert
            Assert.AreEqual(Guid.Parse("41444ccb-9687-4d3a-b0d5-295f3c35b153"), index.Id);
            Assert.AreEqual("test", index.Name);
            Assert.AreEqual(new DateTime(2019, 11, 6, 15, 0, 0), index.DateCreated);
        }

        [Test]
        public async Task ThenWillReturnNullIfNoEntriesFound()
        {
            //Arrange
            var indexLookUpResponse = @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[]}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        "test-reservations-index-registry",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            //Act
            var index = await _repository.GetCurrentReservationIndex();

            //Assert
            Assert.IsNull(index);
        }
    }
}
