using System;
using System.Collections.Generic;
using System.Text;
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

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenFilteringReservations
    {
        private Mock<IElasticLowLevelClient> _mockClient;
        private ReservationsApiEnvironment _apiEnvironment;
        private ReservationIndexRepository _repository;

        [SetUp]
        public void Init()
        {
            _mockClient = new Mock<IElasticLowLevelClient>();
            _apiEnvironment = new ReservationsApiEnvironment("test");
            _repository = new ReservationIndexRepository(_mockClient.Object, _apiEnvironment, Mock.Of<ILogger<ReservationIndexRepository>>());

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
                @"{""took"":1,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,
                ""skipped"":0,""failed"":0},""hits"":{""total"":{""value"":14,""relation"":""eq""},
                ""max_score"":null,""hits"":[]},""aggregations"":{""uniqueCourseDescription"":
                {""doc_count_error_upper_bound"":0,""sum_other_doc_count"":0,""buckets"":[{""key"":
                ""Baker - Level 1"",""doc_count"":4},{""key"":""Banking - Level 2"",""doc_count"":2}]}}}";


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
        public async Task ThenWillSearchForAllCourseTypesAvailable()
        {
            //Arrange
            var expectedQuery = @"{""aggs"":{""uniqueCourseDescription"":
                                  {""terms"":{""field"":""courseDescription.keyword""}}}}";

            //Act
            await _repository.Find(10, "10", 1, 1);

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
            var result = await _repository.Find(10, "10", 1, 1);

            //Assert
            result.Filters.Should().NotBeNull();
            result.Filters.CourseFilters.Should().NotBeNull();
            result.Filters.CourseFilters.Count.Should().Be(2);
            result.Filters.CourseFilters.Should().Contain("Baker - Level 1");
            result.Filters.CourseFilters.Should().Contain("Banking - Level 2");
        }
    }
}
