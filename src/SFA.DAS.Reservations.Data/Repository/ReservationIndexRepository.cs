using System;
using SFA.DAS.Reservations.Domain.Reservations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
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

        public ReservationIndexRepository(IElasticLowLevelClient client, ReservationsApiEnvironment environment)
        {
            _client = client;
            _environment = environment;
        }

        //TODO: Create test for special characters being used and making sure they do not affect search

        public async Task<IndexedReservationSearchResult> Find(long providerId, string term, ushort pageNumber, ushort pageItemCount)
        {
            var data = PostData.String(GetIndexSearchString());

            var response = await _client.SearchAsync<StringResponse>(
               _environment.EnvironmentName + ReservationIndexLookupName, data);

            var elasticResponse = JsonConvert.DeserializeObject<ElasticResponse<IndexRegistryEntry>>(response.Body);

            if (!elasticResponse.Items.Any())
            {
                return new IndexedReservationSearchResult();
            }

            var reservationIndexName = elasticResponse.Items.First().Name;

            ElasticResponse<ReservationIndex> elasticSearchResult;

            if (string.IsNullOrWhiteSpace(reservationIndexName))
            {
                return new IndexedReservationSearchResult();
            }

            var startingDocumentIndex = (ushort) (pageNumber < 2 ? 0 : (pageNumber - 1) * pageItemCount);

            if (string.IsNullOrEmpty(term))
            {
                var request = GetReservationsSearchString(startingDocumentIndex, pageItemCount, providerId);

                var searchRawResponse = await _client.SearchAsync<StringResponse>(reservationIndexName, PostData.String(request));
                elasticSearchResult = JsonConvert.DeserializeObject<ElasticResponse<ReservationIndex>>(searchRawResponse.Body);
            }
            else
            {
                var searchTermWords = term.Split(' ');

                var formattedSearchTerm = searchTermWords.Length > 1
                    ? searchTermWords.Aggregate((x, y) => $"*{x.Trim()}* AND *{y.Trim()}*")
                    : $"*{term}*";

                var request = GetReservationsSearchString(startingDocumentIndex, pageItemCount, providerId, formattedSearchTerm);

                var searchRawResponse = await _client.SearchAsync<StringResponse>(reservationIndexName, PostData.String(request));
                elasticSearchResult = JsonConvert.DeserializeObject<ElasticResponse<ReservationIndex>>(searchRawResponse.Body);
            }

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
            queryBuilder.Append(@"""size"": 0,");
            queryBuilder.Append(@"""sort"":  {""dateCreated"": {""order"": ""desc""}}");

            return queryBuilder.ToString();
        }

        private string GetReservationsSearchString(ushort startingDocumentIndex, ushort pageItemCount, long providerId)
        {
            return @"{""from"": " + startingDocumentIndex + @",""query"":{""bool"":{""must"":[{""match"":{""indexedProviderId"":{""query"":""" + providerId + @"""}}}]}},""size"":" + pageItemCount + @",""sort"":[{
            ""accountLegalEntityName.keyword"":{""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate"":{""order"":""desc""}}]}";
        }

        private string GetReservationsSearchString(ushort startingDocumentIndex, ushort pageItemCount, long providerId, string searchTerm)
        {
            return @"{""from"": " + startingDocumentIndex + @",""query"":{""bool"":{""filter"":[{""query_string"":{""fields"":[""courseName"",""accountLegalEntityName""],
            ""query"":""" + searchTerm + @"""}}],""must"":[{""match"":{""indexedProviderId"":{""query"":""" + providerId + @"""}}}]}},""size"":" + pageItemCount + @",""sort"":[{
            ""accountLegalEntityName.keyword"":{""order"":""asc""}},{""courseTitle.keyword"":{""order"":""asc""}},{""startDate"":{""order"":""desc""}}]}";
        }

        private class IndexRegistryEntry
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
