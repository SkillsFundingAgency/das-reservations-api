using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
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
    public class WhenFilteringReservations
    {
        private Mock<IElasticLowLevelClient> _mockClient;
        private ReservationsApiEnvironment _apiEnvironment;
        private ReservationIndexRepository _repository;
        private SelectedSearchFilters _expectedSelectedFilters;

        [SetUp]
        public void Init()
        {
            _mockClient = new Mock<IElasticLowLevelClient>();
            _apiEnvironment = new ReservationsApiEnvironment("test");
            _repository = new ReservationIndexRepository(_mockClient.Object, _apiEnvironment, Mock.Of<ILogger<ReservationIndexRepository>>());

            _expectedSelectedFilters = new SelectedSearchFilters
            {
                CourseFilter = "Baker - Level 1",
                EmployerNameFilter = "Test Ltd",
                StartDateFilter = DateTime.Now.ToString("dd/MM/yyyy")
            };

            var indexLookUpResponse = @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            ""test"",""dateCreated"":""2019-11-06T15:11:00.5385739+00:00""},""sort"":[1573053060538]}]}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        "test-reservations-index-registry",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));

            var aggregationResponse =
                @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},
                ""hits"":{""total"":{""value"":5,""relation"":""eq""},""max_score"":null,""hits"":[]},""aggregations"":
                {""uniqueStartDate"":{""doc_count_error_upper_bound"":0,""sum_other_doc_count"":0,""buckets"":[{""key"":
                ""2019-09-01 00:00:00"",""doc_count"":2},{""key"":""2019-10-01 00:00:00"",""doc_count"":2}]},""uniqueAccountLegalEntityName"":
                {""doc_count_error_upper_bound"":0,""sum_other_doc_count"":0,""buckets"":[{""key"":""Acme Bank"",""doc_count"":2},
                {""key"":""Test Ltd"",""doc_count"":2}]},""uniqueCourseDescription"":{""doc_count_error_upper_bound"":0,
                ""sum_other_doc_count"":0,""buckets"":[{""key"":""Baker - Level 1"",""doc_count"":4},{""key"":""Banking - Level 2"",""doc_count"":2}]}}}";

            _mockClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        "test",
                        It.Is<PostData>(pd => pd.GetRequestString().Contains("aggs")),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(aggregationResponse));

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
                        "test",
                        It.Is<PostData>(pd => !pd.GetRequestString().Contains("aggs")),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(searchReponse));
        }

        [Test]
        public async Task ThenWillSearchForAllFilterTypesAvailable()
        {
            //Arrange
            var expectedQuery = @"{""aggs"":{""uniqueCourseDescription"":{""terms"":{""field"":""courseDescription.keyword""}},
                                ""uniqueAccountLegalEntityName"":{""terms"":{""field"":""accountLegalEntityName.keyword""}},
                                ""uniqueStartDate"":{""terms"":{""field"":""startDate.keyword""}}}}";

            //Act
            await _repository.Find(10, "10", 1, 1, _expectedSelectedFilters);

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    "test",
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().RemoveLineEndingsAndWhiteSpace()
                            .Equals(expectedQuery.RemoveLineEndingsAndWhiteSpace())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnAllAvailableCourseFilterOptions()
        {
            //Act
            var result = await _repository.Find(10, "10", 1, 1, _expectedSelectedFilters);

            //Assert
            result.Filters.Should().NotBeNull();
            result.Filters.CourseFilters.Should().NotBeNull();
            result.Filters.CourseFilters.Count.Should().Be(2);
            result.Filters.CourseFilters.Should().Contain("Baker - Level 1");
            result.Filters.CourseFilters.Should().Contain("Banking - Level 2");
        }

        
        [Test]
        public async Task ThenShouldReturnAllAvailableEmployerNameFilterOptions()
        {
            //Act
            var result = await _repository.Find(10, "10", 1, 1, _expectedSelectedFilters);

            //Assert
            result.Filters.Should().NotBeNull();
            result.Filters.EmployerFilters.Should().NotBeNull();
            result.Filters.EmployerFilters.Count.Should().Be(2);
            result.Filters.EmployerFilters.Should().Contain("Test Ltd");
            result.Filters.EmployerFilters.Should().Contain("Acme Bank");
        }

        [Test]
        public async Task ThenShouldReturnAllAvailableStartDateFilterOptions()
        {
            //Act
            var result = await _repository.Find(10, "10", 1, 1, _expectedSelectedFilters);

            //Assert
            result.Filters.Should().NotBeNull();
            result.Filters.StartDateFilters.Should().NotBeNull();
            result.Filters.StartDateFilters.Count.Should().Be(2);
            result.Filters.StartDateFilters.Should().Contain("2019-09-01 00:00:00");
            result.Filters.StartDateFilters.Should().Contain("2019-10-01 00:00:00");
        }

        [Test]
        public async Task ThenWillFilterSearchResultsByAllFilters()
        {
            //Arrange
            var expectedSearchTerm = "test";
            var expectedProviderId = 1001;
            ushort pageNumber = 1;
            ushort pageItemSize = 2;

            var query =
                @"{""from"":""0"",""query"":{""bool"":{""should"":[{""match"":{""courseDescription"":
                {""query"":""" + _expectedSelectedFilters.CourseFilter + @""",""operator"":""and""}}},{""match"":{""accountLegalEntityName"":
                {""query"":""" + _expectedSelectedFilters.EmployerNameFilter + @""",""operator"":""and""}}},{""match"":{""startDate"":
                {""query"":""" + _expectedSelectedFilters.StartDateFilter + @""",""operator"":""and""}}}],""minimum_should_match"":3,""must_not"":
                [{""term"":{""status"":{""value"":""3""}}}],
                ""must"":[{""term"":{""indexedProviderId"":{""value"":""" + expectedProviderId + @"""}}},
                {""multi_match"":{""query"":""" + expectedSearchTerm + @""",""type"":""phrase_prefix"",
                ""fields"":[""accountLegalEntityName"",""courseDescription""]}}]}},
                ""size"":""" + pageItemSize + @""",""sort"":[{""accountLegalEntityName.keyword"":
                {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate.keyword"":
                {""order"":""desc""}}]}";

            //Act
            await _repository.Find(expectedProviderId, expectedSearchTerm, pageNumber, pageItemSize, _expectedSelectedFilters);

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    "test",
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().RemoveLineEndingsAndWhiteSpace().Equals(query.RemoveLineEndingsAndWhiteSpace())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenWillFilterSearchResultsByCourse()
        {
            //Arrange
            var expectedSearchTerm = "test";
            var expectedProviderId = 1001;
            ushort pageNumber = 1;
            ushort pageItemSize = 2;

            _expectedSelectedFilters = new SelectedSearchFilters
            {
                CourseFilter = "Baker - Level 1"
            };

            var query =
                @"{""from"":""0"",""query"":{""bool"":{""should"":[{""match"":{""courseDescription"":
                {""query"":""" + _expectedSelectedFilters.CourseFilter + @""",""operator"":""and""}}}],""minimum_should_match"":1,""must_not"":
                [{""term"":{""status"":{""value"":""3""}}}],
                ""must"":[{""term"":{""indexedProviderId"":{""value"":""" + expectedProviderId + @"""}}},
                {""multi_match"":{""query"":""" + expectedSearchTerm + @""",""type"":""phrase_prefix"",
                ""fields"":[""accountLegalEntityName"",""courseDescription""]}}]}},
                ""size"":""" + pageItemSize + @""",""sort"":[{""accountLegalEntityName.keyword"":
                {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate.keyword"":
                {""order"":""desc""}}]}";

            //Act
            await _repository.Find(expectedProviderId, expectedSearchTerm, pageNumber, pageItemSize, _expectedSelectedFilters);

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    "test",
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().RemoveLineEndingsAndWhiteSpace().Equals(query.RemoveLineEndingsAndWhiteSpace())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenWillFilterSearchResultsByEmployerName()
        {
            //Arrange
            var expectedSearchTerm = "test";
            var expectedProviderId = 1001;
            ushort pageNumber = 1;
            ushort pageItemSize = 2;

            _expectedSelectedFilters = new SelectedSearchFilters
            {
                EmployerNameFilter = "Test Ltd"
            };

            var query =
                @"{""from"":""0"",""query"":{""bool"":{""should"":[{""match"":{""accountLegalEntityName"":
                {""query"":""" + _expectedSelectedFilters.EmployerNameFilter + @""",""operator"":""and""}}}],""minimum_should_match"":1,""must_not"":
                [{""term"":{""status"":{""value"":""3""}}}],
                ""must"":[{""term"":{""indexedProviderId"":{""value"":""" + expectedProviderId + @"""}}},
                {""multi_match"":{""query"":""" + expectedSearchTerm + @""",""type"":""phrase_prefix"",
                ""fields"":[""accountLegalEntityName"",""courseDescription""]}}]}},
                ""size"":""" + pageItemSize + @""",""sort"":[{""accountLegalEntityName.keyword"":
                {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate.keyword"":
                {""order"":""desc""}}]}";

            //Act
            await _repository.Find(expectedProviderId, expectedSearchTerm, pageNumber, pageItemSize, _expectedSelectedFilters);

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    "test",
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().RemoveLineEndingsAndWhiteSpace().Equals(query.RemoveLineEndingsAndWhiteSpace())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        public async Task ThenWillFilterSearchResultsByStartDate()
        {
            //Arrange
            var expectedSearchTerm = "test";
            var expectedProviderId = 1001;
            ushort pageNumber = 1;
            ushort pageItemSize = 2;

            _expectedSelectedFilters = new SelectedSearchFilters
            {
                StartDateFilter = DateTime.Now.ToString("dd/MM/yyyy")
            };

            var query =
                @"{""from"":""0"",""query"":{""bool"":{""should"":[{""match"":{""startDate"":
                {""query"":""" + _expectedSelectedFilters.StartDateFilter + @""",""operator"":""and""}}}],""minimum_should_match"":1,""must_not"":
                [{""term"":{""status"":{""value"":""3""}}}],
                ""must"":[{""term"":{""indexedProviderId"":{""value"":""" + expectedProviderId + @"""}}},
                {""multi_match"":{""query"":""" + expectedSearchTerm + @""",""type"":""phrase_prefix"",
                ""fields"":[""accountLegalEntityName"",""courseDescription""]}}]}},
                ""size"":""" + pageItemSize + @""",""sort"":[{""accountLegalEntityName.keyword"":
                {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate.keyword"":
                {""order"":""desc""}}]}";

            //Act
            await _repository.Find(expectedProviderId, expectedSearchTerm, pageNumber, pageItemSize, _expectedSelectedFilters);

            //Assert
            _mockClient.Verify(c =>
                c.SearchAsync<StringResponse>(
                    "test",
                    It.Is<PostData>(pd =>
                        pd.GetRequestString().RemoveLineEndingsAndWhiteSpace().Equals(query.RemoveLineEndingsAndWhiteSpace())),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
