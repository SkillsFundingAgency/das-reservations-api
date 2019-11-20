using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.ElasticSearch;
using SFA.DAS.Reservations.Data.UnitTests.Extensions;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenFindingReservations
    {
        private const string ExpectedEnvironmentName = "test";
        private const string ExpectedReservationIndexLookupName = ExpectedEnvironmentName + "-reservations-index-registry";
        private const string ExpectedLatestReservationIndexName = "test-reservations-35c937c2-f0b1-4a57-9ebb-621a2834ae8b";

        private Mock<IElasticLowLevelClient> _mockClient;
        private ReservationsApiEnvironment _apiEnvironment;
        private ReservationIndexRepository _repository;

        [SetUp]
        public void Init()
        {
            _mockClient = new Mock<IElasticLowLevelClient>();
            _apiEnvironment = new ReservationsApiEnvironment(ExpectedEnvironmentName);
            _repository = new ReservationIndexRepository(_mockClient.Object, _apiEnvironment, Mock.Of<ILogger<ReservationIndexRepository>>());

            var indexLookUpResponse =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            """ + ExpectedLatestReservationIndexName + @""",""dateCreated"":""2019-11-06T15:11:00.5385739+00:00""},""sort"":[1573053060538]}]}}";


            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedReservationIndexLookupName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            var searchReponse =
                @"{""took"":33,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},
                                ""hits"":{""total"":{""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":
                                ""local-reservations-84ede7a8-20da-4d17-b273-ae76c377550f"",""_type"":""_doc"",""_id"":
                                ""2_11_dd7a05b5-7252-44c7-b411-24b92e226977"",""_score"":null,""_source"":{""id"":
                                ""2_11_dd7a05b5-7252-44c7-b411-24b92e226977"",""reservationId"":""dd7a05b5-7252-44c7-b411-24b92e226977"",
                                ""accountId"":1,""isLevyAccount"":false,""createdDate"":""2019-08-20T14:37:01.7530000"",""startDate"":
                                ""2019-09-01T00:00:00"",""expiryDate"":""2020-09-30T00:00:00"",""status"":1,""courseId"":""4"",""courseTitle"":
                                ""Computer Management"",""courseLevel"":2,""courseName"":""Computer Management 2"",""accountLegalEntityId"":11,
                                ""providerId"":2,""accountLegalEntityName"":""Test Ltd"",""indexedProviderId"":2},""sort"":[""Test Co"",
                                ""Computer Management"",1567296000000]}]}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedLatestReservationIndexName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(searchReponse));

            _mockClient.Setup(x => x.CountAsync<StringResponse>(ExpectedLatestReservationIndexName,
                It.IsAny<PostData>(),
                It.IsAny<CountRequestParameters>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new StringResponse(
                @"{""count"":50,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0}}"));
        }

        [Test]
        public async Task ThenWillLookUpLatestReservationList()
        {
            //Arrange
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(@"{""from"": 0,");
            queryBuilder.Append(@"""size"": 1,");
            queryBuilder.Append(@"""sort"":  {""dateCreated"": {""order"": ""desc""}}}");

            //Act
            await _repository.Find(10, "10", 1, 1, new SelectedSearchFilters());

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    ExpectedReservationIndexLookupName,
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().Equals(queryBuilder.ToString())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenWillSearchInTheLatestReservationsIndexUsingTheProviderSearchTerm()
        {
            //Arrange
            var expectedSearchTerm = "test";
            var expectedProviderId = 1001;
            ushort pageNumber = 1;
            ushort pageItemSize = 2;

            var query = @"{""from"":""0"",""query"":{""bool"":{""must_not"":[{""term"":{""status"":{""value"":""3""}}}],""must"":[{""term"":
            {""indexedProviderId"":{""value"":""" + expectedProviderId + @"""}}},{""multi_match"":{""query"":""" + expectedSearchTerm + @""",""type"":""phrase_prefix"",""fields"":
            [""accountLegalEntityName"",""courseDescription""]}}]}},""size"":""" + pageItemSize + @""",""sort"":[{""accountLegalEntityName.keyword"":
            {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""reservationPeriod.keyword"":{""order"":""desc""}}]}";

            //Act
            await _repository.Find(expectedProviderId, expectedSearchTerm, pageNumber, pageItemSize, new SelectedSearchFilters());

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    ExpectedLatestReservationIndexName,
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().RemoveLineEndingsAndWhiteSpace().Equals(query.RemoveLineEndingsAndWhiteSpace())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenWillSearchInTheLatestReservationsIndexWithoutASearchTerm()
        {
            //Arrange
            var expectedProviderId = 1001;
            ushort pageNumber = 1;
            ushort pageItemSize = 2;

            var query = @"{""from"":""0"",""query"":{""bool"":{""must_not"":[{""term"":{""status"":{""value"":""3""}}}],""must"":[{""term"":
            {""indexedProviderId"":{""value"":""" + expectedProviderId + @"""}}}]}},""size"":""" + pageItemSize + @""",""sort"":[{""accountLegalEntityName.keyword"":
            {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""reservationPeriod.keyword"":{""order"":""desc""}}]}";

            //Act
            await _repository.Find(expectedProviderId, string.Empty, pageNumber, pageItemSize, new SelectedSearchFilters());

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    ExpectedLatestReservationIndexName,
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().RemoveLineEndingsAndWhiteSpace().Equals(query.RemoveLineEndingsAndWhiteSpace())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenWillNotSearchReservationsIfLookUpIsEmpty()
        {
            //Arrange
            var indexLookUpResponse =
                @"{""took"":0,""timed_out"":false,""_shards"":{""total"":0,""successful"":1,""skipped"":0,""failed"":0},
                ""hits"":{""total"":{""value"":0,""relation"":""eq""},""max_score"":null,""hits"":[]}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedReservationIndexLookupName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            //Act
            await _repository.Find(10, "10", 1, 1, new SelectedSearchFilters());

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    It.Is<string>(s => !s.Equals(ExpectedReservationIndexLookupName)),
                    It.IsAny<PostData>(),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ThenWillNotSearchReservationsIfLatestReservationIndexHasNoName()
        {
            //Arrange
            var indexLookUpResponse =
                @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            "" "",""dateCreated"":""2019-11-06T15:11:00.5385739+00:00""},""sort"":[1573053060538]}]}}";
            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedReservationIndexLookupName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            //Act
            await _repository.Find(10, "10", 1, 1, new SelectedSearchFilters());

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    It.Is<string>(s => !s.Equals(ExpectedReservationIndexLookupName)),
                    It.IsAny<PostData>(),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ThenWillReturnReservationsFoundWithEmptySearch()
        {
            //Act
            var results = await _repository.Find(2, string.Empty, 1, 1, new SelectedSearchFilters());

            //Assert
            Assert.AreEqual(3, results.TotalReservations);
            Assert.AreEqual(1, results.Reservations.Count());

            var reservation = results.Reservations.First();

            Assert.AreEqual("2_11_dd7a05b5-7252-44c7-b411-24b92e226977", reservation.Id);
            Assert.AreEqual("dd7a05b5-7252-44c7-b411-24b92e226977", reservation.ReservationId.ToString());
            Assert.AreEqual(1, reservation.AccountId);
            Assert.AreEqual(2, reservation.ProviderId);
            Assert.AreEqual(11, reservation.AccountLegalEntityId);
            Assert.AreEqual("Test Ltd", reservation.AccountLegalEntityName);
            Assert.AreEqual("4", reservation.CourseId);
            Assert.AreEqual("Computer Management", reservation.CourseTitle);
            Assert.AreEqual(2, reservation.CourseLevel);
            Assert.AreEqual("01/09/2019", reservation.StartDate.Value.ToString("dd/MM/yyyy"));
            Assert.AreEqual(2, reservation.IndexedProviderId);
            Assert.AreEqual("20/08/2019", reservation.CreatedDate.ToString("dd/MM/yyyy"));
            Assert.AreEqual("30/09/2020", reservation.ExpiryDate.Value.ToString("dd/MM/yyyy"));
            Assert.AreEqual(false, reservation.IsLevyAccount);
            Assert.AreEqual(1, reservation.Status);
        }

        [Test]
        public async Task ThenWillReturnReservationsFoundWithSearchTerm()
        {
            //Act
            var results = await _repository.Find(2, "Test", 1, 1, new SelectedSearchFilters());

            //Assert
            Assert.AreEqual(3, results.TotalReservations);
            Assert.AreEqual(1, results.Reservations.Count());

            var reservation = results.Reservations.First();

            Assert.AreEqual("2_11_dd7a05b5-7252-44c7-b411-24b92e226977", reservation.Id);
            Assert.AreEqual("dd7a05b5-7252-44c7-b411-24b92e226977", reservation.ReservationId.ToString());
            Assert.AreEqual(1, reservation.AccountId);
            Assert.AreEqual(2, reservation.ProviderId);
            Assert.AreEqual(11, reservation.AccountLegalEntityId);
            Assert.AreEqual("Test Ltd", reservation.AccountLegalEntityName);
            Assert.AreEqual("4", reservation.CourseId);
            Assert.AreEqual("Computer Management", reservation.CourseTitle);
            Assert.AreEqual(2, reservation.CourseLevel);
            Assert.AreEqual("01/09/2019", reservation.StartDate.Value.ToString("dd/MM/yyyy"));
            Assert.AreEqual(2, reservation.IndexedProviderId);
            Assert.AreEqual("20/08/2019", reservation.CreatedDate.ToString("dd/MM/yyyy"));
            Assert.AreEqual("30/09/2020", reservation.ExpiryDate.Value.ToString("dd/MM/yyyy"));
            Assert.AreEqual(false, reservation.IsLevyAccount);
            Assert.AreEqual(1, reservation.Status);
        }

        [Test]
        public async Task ThenWillReturnEmptyResultIfReservationIndexLookupReturnInvalidResponse()
        {
            //Arrange
            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedReservationIndexLookupName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(""));


            //Act
            var result = await _repository.Find(1, string.Empty, 1, 10, new SelectedSearchFilters());

            //Assert
            Assert.IsNotNull(result?.Reservations);
            Assert.IsEmpty(result.Reservations);
            Assert.AreEqual(0, result.TotalReservations);
        }

        [Test]
        public async Task ThenWillReturnEmptyResultIfReservationIndexRequestReturnsInvalidResponse()
        {
            //Arrange
            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedLatestReservationIndexName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(""));


            //Act
            var result = await _repository.Find(1, string.Empty, 1, 10, new SelectedSearchFilters());

            //Assert
            Assert.IsNotNull(result?.Reservations);
            Assert.IsEmpty(result.Reservations);
            Assert.AreEqual(0, result.TotalReservations);
        }

        [Test]
        public async Task ThenWillReturnEmptyResultIfReservationIndexLookupReturnFailedResponse()
        {
            //Arrange
            var response =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":0,""skipped"":0,""failed"":1},""hits"":{""total"":
            {""value"":0,""relation"":""eq""},""max_score"":null,""hits"":[]}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedReservationIndexLookupName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(response));

            //Act
            var result = await _repository.Find(1, string.Empty, 1, 10, new SelectedSearchFilters());

            //Assert
            Assert.IsNotNull(result?.Reservations);
            Assert.IsEmpty(result.Reservations);
            Assert.AreEqual(0, result.TotalReservations);

            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    It.Is<string>(s => !s.Equals(ExpectedReservationIndexLookupName)),
                    It.IsAny<PostData>(),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ThenWillReturnEmptyResultIfReservationIndexRequestReturnsFailedResponse()
        {
            //Arrange
            var response =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":0,""skipped"":0,""failed"":1},""hits"":{""total"":
            {""value"":0,""relation"":""eq""},""max_score"":null,""hits"":[]}}";


            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        ExpectedLatestReservationIndexName,
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(response));

            //Act
            var result = await _repository.Find(1, string.Empty, 1, 10, new SelectedSearchFilters());

            //Assert
            Assert.IsNotNull(result?.Reservations);
            Assert.IsEmpty(result.Reservations);
            Assert.AreEqual(0, result.TotalReservations);
        }
    }
}
