using System;
using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Reservations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Data.ElasticSearch;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        public const string ReservationIndexLookupName = "-reservations-index-registry";

        private readonly IElasticLowLevelClient _client;
        private readonly ReservationsApiEnvironment _environment;
        private readonly ILogger<ReservationIndexRepository> _logger;

        public ReservationIndexRepository(IElasticLowLevelClient client, ReservationsApiEnvironment environment, ILogger<ReservationIndexRepository> logger)
        {
            _client = client;
            _environment = environment;
            _logger = logger;
        }

        public async Task<IndexedReservationSearchResult> Find(
            long providerId, string searchTerm, ushort pageNumber, ushort pageItemCount, SearchFilters selectedFilters)
        {
            _logger.LogInformation("Starting reservation search");

            var reservationIndexName = await GetCurrentReservationIndexName();

            if (string.IsNullOrWhiteSpace(reservationIndexName))
            {
                _logger.LogWarning("Searching failed. Latest Reservation index does not have a name value");

                return new IndexedReservationSearchResult();
            }

            var startingDocumentIndex = (ushort) (pageNumber < 2 ? 0 : (pageNumber - 1) * pageItemCount);

            var elasticSearchResult = await GetSearchResult(
                providerId, searchTerm, pageItemCount, startingDocumentIndex, reservationIndexName, selectedFilters);

            if (elasticSearchResult == null)
            {
                _logger.LogWarning("Searching failed. Elastic search response could not be de-serialised");
                return new IndexedReservationSearchResult();
            }

            _logger.LogDebug("Searching complete, returning search results");

            var courseFilterValues = await GetCourseFilterValues(reservationIndexName);

            var searchResult =  new IndexedReservationSearchResult
            {
               Reservations = elasticSearchResult.Items,
               TotalReservations = (uint) elasticSearchResult.hits.total.value,
               Filters = new SearchFilters { CourseFilters = courseFilterValues}
            };

            return searchResult;
        }

        private async Task<ICollection<string>> GetCourseFilterValues(string reservationIndexName)
        {
            var request = GetCourseFilterValuesQuery();
            
            var jsonResponse =
                await _client.SearchAsync<StringResponse>(reservationIndexName, PostData.String(request));
            
            var response = JsonConvert.DeserializeObject<ElasticResponse<ReservationIndex>>(jsonResponse.Body);

            var courseFilterValues = response.aggregations?.uniqueCourseDescription?.buckets?.Select(b => b.key).ToList();
            
            return courseFilterValues ?? new List<string>();
        }

        private async Task<ElasticResponse<ReservationIndex>> GetSearchResult(
            long providerId, string searchTerm, ushort pageItemCount,
            ushort startingDocumentIndex, string reservationIndexName, 
            SearchFilters selectedFilters)
        {
            var request = string.IsNullOrEmpty(searchTerm) ?
                GetReservationsSearchString(startingDocumentIndex, pageItemCount, providerId, selectedFilters) :
                GetReservationsSearchString(startingDocumentIndex, pageItemCount, providerId, searchTerm, selectedFilters);

            _logger.LogDebug($"Searching with search term: {searchTerm}");

            var jsonResponse =
                await _client.SearchAsync<StringResponse>(reservationIndexName, PostData.String(request));

            var searchResult = JsonConvert.DeserializeObject<ElasticResponse<ReservationIndex>>(jsonResponse.Body);

            return searchResult;
        }

        private async Task<string> GetCurrentReservationIndexName()
        {
            var data = PostData.String(GetIndexSearchString());

            _logger.LogDebug("Getting latest reservation index name");

            var response = await _client.SearchAsync<StringResponse>(
                _environment.EnvironmentName + ReservationIndexLookupName, data);

            var elasticResponse = JsonConvert.DeserializeObject<ElasticResponse<IndexRegistryEntry>>(response.Body);

            if (elasticResponse?.Items != null && elasticResponse.Items.Any())
            {
                return elasticResponse.Items.First().Name;
            }

            _logger.LogWarning("Searching failed. Could not find any reservation index names to search");

            return null;
        }

        private string GetCourseFilterValuesQuery()
        {
            return @"{""aggs"":{""uniqueCourseDescription"":
                    { ""terms"":{ ""field"":""courseDescription.keyword""} }}}";
        }

        private string GetIndexSearchString()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(@"{""from"": 0,");
            queryBuilder.Append(@"""size"": 1,");
            queryBuilder.Append(@"""sort"":  {""dateCreated"": {""order"": ""desc""}}}");

            return queryBuilder.ToString();
        }

        private string GetReservationsSearchString(
            ushort startingDocumentIndex, ushort pageItemCount, long providerId, SearchFilters selectedFilters)
        {
            var filterClause = string.Empty;

            if (selectedFilters.HasFilters)
            {
                filterClause = GetFilterSearchSubString(selectedFilters);
            }

            return @"{""from"":""" + startingDocumentIndex + @""",""query"":{""bool"":{" + filterClause + @"""must_not"":[{""term"":{""status"":{""value"":""3""}}}],""must"":[{""term"":
            {""indexedProviderId"":{""value"":""" + providerId + @"""}}}]}},""size"":""" + pageItemCount + @""",""sort"":[{""accountLegalEntityName.keyword"":
            {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate"":{""order"":""desc""}}]}";
        }

        private string GetReservationsSearchString(
            ushort startingDocumentIndex, ushort pageItemCount, long providerId, string searchTerm, SearchFilters selectedFilters)
        {
            var filterClause = string.Empty;

            if (selectedFilters.HasFilters)
            {
                filterClause = GetFilterSearchSubString(selectedFilters);
            }

            return @"{""from"":""" + startingDocumentIndex + @""",""query"":{""bool"":{" + filterClause + @"""must_not"":[{""term"":{""status"":{""value"":""3""}}}],""must"":[{""term"":
            {""indexedProviderId"":{""value"":""" + providerId + @"""}}},{""multi_match"":{""query"":""" + searchTerm + @""",""type"":""phrase_prefix"",""fields"":
            [""accountLegalEntityName"",""courseDescription""]}}]}},""size"":""" + pageItemCount + @""",""sort"":[{""accountLegalEntityName.keyword"":
            {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate"":{""order"":""desc""}}]}";
        }

        private string GetFilterSearchSubString(SearchFilters selectedFilters)
        {
            var filterClauseBuilder = new StringBuilder();

            foreach (var courseFilterTerm in selectedFilters.CourseFilters)
            {
                filterClauseBuilder.Append(@"{""term"" : { ""courseDescription"" : """ + courseFilterTerm + @"""}},");
            }

            var filterClause = filterClauseBuilder.ToString().TrimEnd(',');

            return @"""should"": [" + filterClause + @"], ""minimum_should_match"": 1,";
        }

        private class IndexRegistryEntry
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
