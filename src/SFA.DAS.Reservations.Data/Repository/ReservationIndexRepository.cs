using System;
using SFA.DAS.Reservations.Domain.Reservations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        public const string ReservationIndexLookupName = "-reservations-index-registry";

        private readonly IElasticClient _client;
        private readonly ReservationsApiEnvironment _environment;

        public ReservationIndexRepository(IElasticClient client, ReservationsApiEnvironment environment)
        {
            _client = client;
            _environment = environment;
        }

        public async Task<IEnumerable<ReservationIndex>> Find(long providerId, string term)
        {
            var searchIndexRegistryResponse = _client.Search<IndexRegistryEntry>(s => s
                .Index(_environment.EnvironmentName + ReservationIndexLookupName)
                .From(0)
                .Size(1)
                .Sort(x => x.Descending(a => a.DateCreated)));

            if (searchIndexRegistryResponse?.Documents == null || 
                !searchIndexRegistryResponse.Documents.Any())
            {
                return new ReservationIndex[0];
            }

            var reservationIndexName = searchIndexRegistryResponse.Documents.First().Name;

            if (string.IsNullOrEmpty(reservationIndexName))
            {
                return new ReservationIndex[0];
            }

            ISearchResponse<ReservationIndex> searchResponse;

            if (string.IsNullOrEmpty(term))
            {
                searchResponse = await _client.SearchAsync<ReservationIndex>(s => s
                    .Index(reservationIndexName)
                    .From(0)
                    .Size(100)
                    .Query(q =>
                        q.Bool(b => b
                            .Must(x => x.Match(m => m.Field(f => f.IndexedProviderId).Query(providerId.ToString())))
                        )));
            }
            else
            {
                var searchTermWords = term.Split(' ');

                var formattedSearchTerm = searchTermWords.Length > 1
                    ? searchTermWords.Aggregate((x, y) => $"*{x.Trim()}* AND *{y.Trim()}*")
                    : $"*{term}*";

                searchResponse = await _client.SearchAsync<ReservationIndex>(s => s
                    .Index(reservationIndexName)
                    .From(0)
                    .Size(100)
                    .Query(q =>
                        q.Bool(b => b
                            .Must(x => x.Match(m => m.Field(f => f.IndexedProviderId).Query(providerId.ToString())))
                            .Filter(x => x.QueryString(descriptor => descriptor
                                .Fields(fields => fields.Field(f => f.CourseName)
                                    .Field(f => f.AccountLegalEntityName))
                                .Query(formattedSearchTerm)))
                        )));
            }

            return searchResponse.Documents;
        }

        private class IndexRegistryEntry
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
