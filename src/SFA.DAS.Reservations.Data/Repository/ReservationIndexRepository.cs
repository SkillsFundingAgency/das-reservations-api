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
        private readonly IElasticSearchQueries _elasticQueries;
        private readonly ILogger<ReservationIndexRepository> _logger;

        public ReservationIndexRepository(IElasticLowLevelClient client, ReservationsApiEnvironment environment, IElasticSearchQueries elasticQueries, ILogger<ReservationIndexRepository> logger)
        {
            _client = client;
            _environment = environment;
            _elasticQueries = elasticQueries;
            _logger = logger;
        }

        public async Task<IndexedReservationSearchResult> Find(
            long providerId, string searchTerm, ushort pageNumber, ushort pageItemCount, SelectedSearchFilters selectedFilters)
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

            var filterValues = await GetFilterValues(reservationIndexName, providerId);

            var searchResult =  new IndexedReservationSearchResult
            {
               Reservations = elasticSearchResult.Items,
               TotalReservations = (uint) elasticSearchResult.hits.total.value,
               Filters = new SearchFilters
               {
                   CourseFilters = filterValues.Courses,
                   EmployerFilters = filterValues.AccountLegalEntityNames,
                   StartDateFilters = filterValues.StartDates
               }
            };

            return searchResult;
        }

        private async Task<FilterValues> GetFilterValues(string reservationIndexName, long providerId)
        {
            var request = GetFilterValuesQuery(providerId);
            
            var jsonResponse =
                await _client.SearchAsync<StringResponse>(reservationIndexName, PostData.String(request));
            
            var response = JsonConvert.DeserializeObject<ElasticResponse<ReservationIndex>>(jsonResponse.Body);

            var coursefilterValues = response.aggregations?.uniqueCourseDescription?.buckets?.Select(b => b.key).ToList();
            var accountLegalEntityfilterValues = response.aggregations?.uniqueAccountLegalEntityName?.buckets?.Select(b => b.key).ToList();
            var startDateFilterValues = response.aggregations?.uniqueReservationPeriod?.buckets?.Select(b => b.key).ToList();
            
            return new FilterValues
            {
                Courses = coursefilterValues,
                AccountLegalEntityNames = accountLegalEntityfilterValues,
                StartDates = startDateFilterValues
            };
        }

        private async Task<ElasticResponse<ReservationIndex>> GetSearchResult(
            long providerId, string searchTerm, ushort pageItemCount,
            ushort startingDocumentIndex, string reservationIndexName,
            SelectedSearchFilters selectedFilters)
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
            var data = PostData.String(_elasticQueries.LastIndexSearchQuery);

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

        private string GetFilterValuesQuery(long providerId)
        {
            return _elasticQueries.GetFilterValuesQuery.Replace("{providerId}", providerId.ToString());
        }

        private string GetReservationsSearchString(
            ushort startingDocumentIndex, ushort pageItemCount, long providerId, SelectedSearchFilters selectedFilters)
        {
            var query = _elasticQueries.GetAllReservationsQuery.Replace("{startingDocumentIndex}", startingDocumentIndex.ToString());
            query = query.Replace("{providerId}", providerId.ToString());
            query = query.Replace("{pageItemCount}", pageItemCount.ToString());

            if (selectedFilters.HasFilters)
            {
                var filterClause = GetFilterSearchSubString(selectedFilters);
                query = query.Replace(@"""should"": []", @"""should"": [" + filterClause + @"]");
            }

            query = query.Replace("{pageItemCount}", pageItemCount.ToString());

            return query;
        }

        private string GetReservationsSearchString(
            ushort startingDocumentIndex, ushort pageItemCount, long providerId, string searchTerm, SelectedSearchFilters selectedFilters)
        {
            var query = _elasticQueries.FindReservationsQuery.Replace("{startingDocumentIndex}", startingDocumentIndex.ToString());
            query = query.Replace("{providerId}", providerId.ToString());
            query = query.Replace("{pageItemCount}", pageItemCount.ToString());
            query = query.Replace("{searchTerm}", searchTerm);

            if (selectedFilters.HasFilters)
            {
                var filterClause = GetFilterSearchSubString(selectedFilters);
                query = query.Replace(@"""should"": []", filterClause);
            }

            return query;
        }

        private string GetFilterSearchSubString(SelectedSearchFilters selectedFilters)
        {
            var filterClauseBuilder = new StringBuilder();
            var minMatchValue = 0;

            if(!string.IsNullOrWhiteSpace(selectedFilters.CourseFilter))
            {
                filterClauseBuilder.Append(@"{""match"" : { ""courseDescription"" : { 
                                              ""query"":""" + selectedFilters.CourseFilter + 
                                              @""", ""operator"":""and""}}},");

                minMatchValue++;
            }

            if(!string.IsNullOrWhiteSpace(selectedFilters.EmployerNameFilter))
            {
                filterClauseBuilder.Append(@"{""match"" : { ""accountLegalEntityName"" : { 
                                              ""query"":""" + selectedFilters.EmployerNameFilter + 
                                           @""", ""operator"":""and""}}},");

                minMatchValue++;
            }

            if(!string.IsNullOrWhiteSpace(selectedFilters.StartDateFilter))
            {
                filterClauseBuilder.Append(@"{""match"" : { ""reservationPeriod"" : { 
                                              ""query"":""" + selectedFilters.StartDateFilter + 
                                           @""", ""operator"":""and""}}},");

                minMatchValue++;
            }

            var filterClause = filterClauseBuilder.ToString().TrimEnd(',');

            return @"""should"": [" + filterClause + @"], ""minimum_should_match"": " + minMatchValue + ",";
        }

        private class IndexRegistryEntry
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
        }

        private struct FilterValues
        {
            public ICollection<string> Courses { get; set; }
            public ICollection<string> AccountLegalEntityNames { get; set; }
            public ICollection<string> StartDates { get; set; }
        }
    }
}
