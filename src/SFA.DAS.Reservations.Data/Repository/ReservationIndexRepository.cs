using System;
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

        public async Task<IndexedReservationSearchResult> Find(long providerId, string searchTerm, ushort pageNumber, ushort pageItemCount)
        {
            _logger.LogInformation("Starting reservation search");

            var data = PostData.String(GetIndexSearchString());

            _logger.LogDebug("Getting latest reservation index name");

            var response = await _client.SearchAsync<StringResponse>(
                _environment.EnvironmentName + ReservationIndexLookupName, data);
           
            var elasticResponse = JsonConvert.DeserializeObject<ElasticResponse<IndexRegistryEntry>>(response.Body);

            if (elasticResponse?.Items == null || !elasticResponse.Items.Any())
            {
                _logger.LogWarning("Searching failed. Could not find any reservation index names to search");

                return new IndexedReservationSearchResult();
            }

            var reservationIndexName = elasticResponse.Items.First().Name;

            ElasticResponse<ReservationIndex> elasticSearchResult;

            if (string.IsNullOrWhiteSpace(reservationIndexName))
            {
                _logger.LogWarning("Searching failed. Latest Reservation index does not have a name value");

                return new IndexedReservationSearchResult();
            }

            var startingDocumentIndex = (ushort) (pageNumber < 2 ? 0 : (pageNumber - 1) * pageItemCount);

            if (string.IsNullOrEmpty(searchTerm))
            {
                _logger.LogDebug("Searching without search term");

                var request = GetReservationsSearchString(startingDocumentIndex, pageItemCount, providerId);

                var searchRawResponse = await _client.SearchAsync<StringResponse>(reservationIndexName, PostData.String(request));

                elasticSearchResult = JsonConvert.DeserializeObject<ElasticResponse<ReservationIndex>>(searchRawResponse.Body);
            }
            else
            {
                _logger.LogDebug("Searching with search term");

                var request = GetReservationsSearchString(startingDocumentIndex, pageItemCount, providerId, searchTerm);

                var searchRawResponse = await _client.SearchAsync<StringResponse>(reservationIndexName, PostData.String(request));

                elasticSearchResult = JsonConvert.DeserializeObject<ElasticResponse<ReservationIndex>>(searchRawResponse.Body);
            }

            if (elasticSearchResult == null)
            {
                _logger.LogWarning("Searching failed. Elastic search response could not be de-serialised");

                return new IndexedReservationSearchResult();
            }

            _logger.LogDebug("Searching complete, returning search results");

            return new IndexedReservationSearchResult
            {
               Reservations = elasticSearchResult.Items,
               TotalReservations = (uint) elasticSearchResult.hits.total.value
            }; 
        }

        private string GetIndexSearchString()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(@"{""from"": 0,");
            queryBuilder.Append(@"""size"": 1,");
            queryBuilder.Append(@"""sort"":  {""dateCreated"": {""order"": ""desc""}}}");

            return queryBuilder.ToString();
        }

        private string GetReservationsSearchString(ushort startingDocumentIndex, ushort pageItemCount, long providerId)
        {
            return @"{""from"":""" + startingDocumentIndex + @""",""query"":{""bool"":{""must_not"":[{""term"":{""status"":{""value"":""3""}}}],""must"":[{""term"":
            {""indexedProviderId"":{""value"":""" + providerId + @"""}}}]}},""size"":""" + pageItemCount + @""",""sort"":[{""accountLegalEntityName.keyword"":
            {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate"":{""order"":""desc""}}]}";
        }

        private string GetReservationsSearchString(ushort startingDocumentIndex, ushort pageItemCount, long providerId, string searchTerm)
        {
            return @"{""from"":""" + startingDocumentIndex + @""",""query"":{""bool"":{""must_not"":[{""term"":{""status"":{""value"":""3""}}}],""must"":[{""term"":
            {""indexedProviderId"":{""value"":""" + providerId + @"""}}},{""multi_match"":{""query"":""" + searchTerm + @""",""type"":""phrase_prefix"",""fields"":
            [""accountLegalEntityName"",""courseDescription""]}}]}},""size"":""" + pageItemCount + @""",""sort"":[{""accountLegalEntityName.keyword"":
            {""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate"":{""order"":""desc""}}]}";
        }

        private class IndexRegistryEntry
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
